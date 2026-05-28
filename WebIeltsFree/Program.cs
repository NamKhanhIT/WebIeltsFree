using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using WebIeltsFree.Models;
using WebIeltsFree.Middleware;
using WebIeltsFree.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

static Uri ResolvePythonSpeakingWsUri(IConfiguration configuration)
{
    var baseUrlTemplate = configuration["PythonAI:BaseUrl"];
    var baseUrl = EnvHelper.ResolveEnvVars(baseUrlTemplate);
    if (string.IsNullOrWhiteSpace(baseUrl))
        baseUrl = "http://localhost:8000";

    if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
        baseUri = new Uri("http://localhost:8000");

    var wsScheme = baseUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? "wss" : "ws";
    var builder = new UriBuilder(baseUri)
    {
        Scheme = wsScheme,
        Path = "/ws/speaking",
        Query = string.Empty
    };

    if (baseUri.IsDefaultPort)
        builder.Port = wsScheme == "wss" ? 443 : 80;

    return builder.Uri;
}

static async Task RelayWebSocketAsync(WebSocket source, WebSocket destination, CancellationToken cancellationToken)
{
    var buffer = new byte[16 * 1024];
    while (!cancellationToken.IsCancellationRequested &&
           source.State == WebSocketState.Open &&
           destination.State == WebSocketState.Open)
    {
        var result = await source.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            if (destination.State == WebSocketState.Open || destination.State == WebSocketState.CloseReceived)
            {
                await destination.CloseOutputAsync(
                    result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    result.CloseStatusDescription,
                    cancellationToken);
            }
            break;
        }

        await destination.SendAsync(
            new ArraySegment<byte>(buffer, 0, result.Count),
            result.MessageType,
            result.EndOfMessage,
            cancellationToken);
    }
}

builder.Services.AddControllersWithViews();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

builder.Services.AddSingleton<IGeminiService, GeminiService>();
builder.Services.Configure<PythonAiOptions>(builder.Configuration.GetSection("PythonAI"));
builder.Services.AddHttpClient<IPythonAiService, PythonAiService>();
builder.Services.AddSingleton<ISpacedRepetitionService, SpacedRepetitionService>();
builder.Services.AddSingleton<IChatbotService, ChatbotService>();

builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IVectorSearchService, VectorSearchService>();

// Background services
builder.Services.AddHostedService<WebIeltsFree.Services.OverdueAssignmentService>();

var redisEnv = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
if (!string.IsNullOrEmpty(redisEnv))
{
    builder.Services.AddSingleton<ICacheService, RedisService>();
}
else
{
    builder.Services.AddSingleton<ICacheService, CacheService>();
}

var connectionString = EnvHelper.ResolveEnvVars(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

// ── Bare-minimum Cookie Auth for development ──
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath  = "/Account/Login";
    options.Cookie.Name = "Dev.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // works on both http and https
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = false; 
});

// Role-based authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOrAdmin", policy =>
        policy.RequireRole("teacher", "admin"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IELTS Learning Platform API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        var allowedOrigins = builder.Configuration["Security:AllowedOrigins"]
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries) 
            ?? new[] { "https://webieltsfree.com" };

        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseSecurityHeaders(); // Security headers in production
}

app.UseRequestLogging();
app.UseRateLimiting();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseWebSockets();

var corsPolicy = app.Environment.IsDevelopment() ? "AllowAll" : "Production";
app.UseCors(corsPolicy);

// DevMagicLoginMiddleware has been removed — app always starts unauthenticated.

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.Map("/ws/speaking", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("WebSocket request expected.");
        return;
    }

    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("SpeakingWsProxy");
    var targetUri = ResolvePythonSpeakingWsUri(context.RequestServices.GetRequiredService<IConfiguration>());

    using var upstream = new ClientWebSocket();
    using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);

    try
    {
        await upstream.ConnectAsync(targetUri, cts.Token);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to connect to Python speaking websocket at {TargetUri}", targetUri);
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsync("Python AI speaking websocket is unavailable.");
        return;
    }

    using var downstream = await context.WebSockets.AcceptWebSocketAsync();

    var downstreamToUpstream = RelayWebSocketAsync(downstream, upstream, cts.Token);
    var upstreamToDownstream = RelayWebSocketAsync(upstream, downstream, cts.Token);

    await Task.WhenAny(downstreamToUpstream, upstreamToDownstream);
    cts.Cancel();

    if (downstream.State == WebSocketState.Open || downstream.State == WebSocketState.CloseReceived)
    {
        await downstream.CloseAsync(WebSocketCloseStatus.NormalClosure, "Proxy closing", CancellationToken.None);
    }
});

app.MapControllers();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
