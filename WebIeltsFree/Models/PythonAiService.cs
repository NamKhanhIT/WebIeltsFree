using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebIeltsFree.Models;

namespace WebIeltsFree.Services;

public class PythonAiOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8000";
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}

public interface IPythonAiService
{
    Task<WritingEvaluation> EvaluateWritingAsync(string essay, int taskType, string? prompt = null, CancellationToken cancellationToken = default);
    Task<SpeakingEvaluation> EvaluateSpeakingAsync(string transcript, string topic, CancellationToken cancellationToken = default);
    Task<string> ChatAsync(string message, List<ChatMessage>? history = null, CancellationToken cancellationToken = default);
}

public class PythonAiService : IPythonAiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PythonAiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PythonAiService(HttpClient httpClient, IOptions<PythonAiOptions> options, ILogger<PythonAiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var cfg = options.Value;
        var resolvedBaseUrl = EnvHelper.ResolveEnvVars(cfg.BaseUrl)?.Trim();
        if (string.IsNullOrWhiteSpace(resolvedBaseUrl))
            resolvedBaseUrl = "http://localhost:8000";

        try
        {
            _httpClient.BaseAddress = new Uri(resolvedBaseUrl.TrimEnd('/') + "/");
        }
        catch (UriFormatException ex)
        {
            _logger.LogWarning(ex, "Invalid PythonAI:BaseUrl '{BaseUrl}', fallback to localhost", resolvedBaseUrl);
            _httpClient.BaseAddress = new Uri("http://localhost:8000/");
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(Math.Max(5, cfg.TimeoutSeconds));

        var resolvedApiKey = EnvHelper.ResolveEnvVars(cfg.ApiKey);
        if (!string.IsNullOrWhiteSpace(resolvedApiKey))
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", resolvedApiKey);
    }

    public async Task<WritingEvaluation> EvaluateWritingAsync(string essay, int taskType, string? prompt = null, CancellationToken cancellationToken = default)
    {
        var request = new PythonWritingEvaluationRequest
        {
            EssayText = essay,
            TaskType = taskType,
            Prompt = prompt
        };

        using var response = await _httpClient.PostAsJsonAsync("v1/writing/evaluate", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Python AI writing evaluate failed ({(int)response.StatusCode}): {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<PythonWritingEvaluationResponse>(JsonOptions, cancellationToken);
        if (result == null)
            throw new InvalidOperationException("Python AI writing evaluate returned empty response");

        return new WritingEvaluation
        {
            OverallBand = result.OverallBand,
            TaskAchievement = result.TaskAchievement,
            CoherenceCohesion = result.CoherenceCohesion,
            LexicalResource = result.LexicalResource,
            GrammaticalRange = result.GrammaticalRange,
            Feedback = result.Feedback ?? string.Empty,
            Strengths = result.Strengths ?? new List<string>(),
            Improvements = result.Improvements ?? new List<string>(),
            GrammarErrors = new List<GrammarError>()
        };
    }

    public async Task<SpeakingEvaluation> EvaluateSpeakingAsync(string transcript, string topic, CancellationToken cancellationToken = default)
    {
        var request = new PythonSpeakingEvaluationRequest
        {
            Transcript = transcript,
            Topic = topic
        };

        using var response = await _httpClient.PostAsJsonAsync("v1/speaking/evaluate", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Python AI speaking evaluate failed ({(int)response.StatusCode}): {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<PythonSpeakingEvaluationResponse>(JsonOptions, cancellationToken);
        if (result == null)
            throw new InvalidOperationException("Python AI speaking evaluate returned empty response");

        return new SpeakingEvaluation
        {
            OverallBand = result.OverallBand,
            Fluency = result.Fluency,
            Pronunciation = result.Pronunciation,
            LexicalResource = result.LexicalResource,
            GrammaticalRange = result.GrammaticalRange,
            Feedback = result.Feedback ?? string.Empty,
            Strengths = result.Strengths ?? new List<string>(),
            Improvements = result.Improvements ?? new List<string>()
        };
    }

    public async Task<string> ChatAsync(string message, List<ChatMessage>? history = null, CancellationToken cancellationToken = default)
    {
        var request = new { message, history = history?.Select(h => new { h.Role, h.Content }).ToList() };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("v1/chat", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Python AI chat failed ({StatusCode}): {Body}", (int)response.StatusCode, body);
                return "I'm sorry, I couldn't process your request right now. Please try again later.";
            }

            var result = await response.Content.ReadFromJsonAsync<PythonChatResponse>(JsonOptions, cancellationToken);
            return result?.Response ?? "No response from AI service.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Python AI chat request failed");
            return "AI service is currently unavailable. Please try again later.";
        }
    }

    private sealed class PythonChatResponse
    {
        public string? Response { get; set; }
    }

    private sealed class PythonWritingEvaluationRequest
    {
        public string EssayText { get; set; } = string.Empty;
        public int TaskType { get; set; }
        public string? Prompt { get; set; }
    }

    private sealed class PythonWritingEvaluationResponse
    {
        public float OverallBand { get; set; }
        public float TaskAchievement { get; set; }
        public float CoherenceCohesion { get; set; }
        public float LexicalResource { get; set; }
        public float GrammaticalRange { get; set; }
        public string? Feedback { get; set; }
        public List<string>? Strengths { get; set; }
        public List<string>? Improvements { get; set; }
    }

    private sealed class PythonSpeakingEvaluationRequest
    {
        public string Transcript { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
    }

    private sealed class PythonSpeakingEvaluationResponse
    {
        public float OverallBand { get; set; }
        public float Fluency { get; set; }
        public float Pronunciation { get; set; }
        public float LexicalResource { get; set; }
        public float GrammaticalRange { get; set; }
        public string? Feedback { get; set; }
        public List<string>? Strengths { get; set; }
        public List<string>? Improvements { get; set; }
    }

}

