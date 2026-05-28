using System.Text;
using System.Text.Json;
using WebIeltsFree.Models;

namespace WebIeltsFree.Services;
public interface IGeminiService
{
    Task<string> GenerateContentAsync(string prompt, string? systemInstruction = null);
    Task<WritingEvaluation> EvaluateWritingAsync(string essay, string taskType);
    Task<SpeakingEvaluation> EvaluateSpeakingAsync(string transcript, string topic);
    Task<string> ChatAsync(string message, List<ChatMessage>? history = null);
}

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly ILogger<GeminiService> _logger;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

    public GeminiService(IConfiguration config, ILogger<GeminiService> logger)
    {
        _httpClient = new HttpClient();
        _apiKey = EnvHelper.ResolveEnvVars(config["Gemini:ApiKey"]) ?? "";
        _model = EnvHelper.ResolveEnvVars(config["Gemini:Model"]) ?? "gemini-1.5-flash";
        _logger = logger;
    }


    public async Task<string> GenerateContentAsync(string prompt, string? systemInstruction = null)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("Gemini API key not configured, using mock response");
            return GetMockResponse(prompt);
        }

        try
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                systemInstruction = systemInstruction != null ? new
                {
                    parts = new[] { new { text = systemInstruction } }
                } : null,
                generationConfig = new
                {
                    temperature = 0.7,
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 2048
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
                }
            };

            var url = $"{BaseUrl}/{_model}:generateContent?key={_apiKey}";
            var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API error: {Response}", jsonResponse);
                return GetMockResponse(prompt);
            }

            using var doc = JsonDocument.Parse(jsonResponse);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? GetMockResponse(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini API error");
            return GetMockResponse(prompt);
        }
    }

    public async Task<WritingEvaluation> EvaluateWritingAsync(string essay, string taskType)
    {
        var systemInstruction = @"You are an expert IELTS examiner. Evaluate essays strictly according to official IELTS Writing band descriptors.
                                Always respond with valid JSON only, no markdown formatting.";

        var prompt = $@"Evaluate this IELTS {taskType} essay and provide scores based on official IELTS band descriptors.
                    Essay:
                    {essay}

                    Return ONLY valid JSON in this exact format:
                    {{
                    ""overallBand"": 6.5,
                    ""taskAchievement"": 6.5,
                    ""coherenceCohesion"": 6.0,
                    ""lexicalResource"": 6.5,
                    ""grammaticalRange"": 6.0,
                    ""feedback"": ""Your detailed feedback here"",
                    ""strengths"": [""strength 1"", ""strength 2""],
                    ""improvements"": [""improvement 1"", ""improvement 2""],
                    ""grammarErrors"": [
                        {{""text"": ""error text"", ""correction"": ""correct text"", ""explanation"": ""why""}}
                    ]
                    }}";

        var response = await GenerateContentAsync(prompt, systemInstruction);
        
        try
        {
            // Clean JSON response (remove markdown code blocks if present)
            response = CleanJsonResponse(response);
            return JsonSerializer.Deserialize<WritingEvaluation>(response, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? GetMockWritingEval();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse writing evaluation, using mock");
            return GetMockWritingEval();
        }
    }

    public async Task<SpeakingEvaluation> EvaluateSpeakingAsync(string transcript, string topic)
    {
        var systemInstruction = @"You are an expert IELTS examiner. Evaluate speaking responses according to official IELTS Speaking band descriptors.
                                Always respond with valid JSON only, no markdown formatting.";

        var prompt = $@"Evaluate this IELTS Speaking response on the topic: {topic}
                    Transcript:
                    {transcript}

                    Return ONLY valid JSON in this exact format:
                    {{
                    ""overallBand"": 6.5,
                    ""fluency"": 6.5,
                    ""pronunciation"": 6.0,
                    ""lexicalResource"": 6.5,
                    ""grammaticalRange"": 6.0,
                    ""feedback"": ""Your detailed feedback here"",
                    ""strengths"": [""strength 1"", ""strength 2""],
                    ""improvements"": [""improvement 1"", ""improvement 2""]
                    }}";

        var response = await GenerateContentAsync(prompt, systemInstruction);
        
        try
        {
            response = CleanJsonResponse(response);
            return JsonSerializer.Deserialize<SpeakingEvaluation>(response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? GetMockSpeakingEval();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse speaking evaluation, using mock");
            return GetMockSpeakingEval();
        }
    }

    public async Task<string> ChatAsync(string message, List<ChatMessage>? history = null)
    {
        var systemInstruction = @"You are an IELTS AI tutor named 'IELTS Buddy'. You help students prepare for the IELTS exam.
                                Be friendly, encouraging, and provide accurate IELTS-related advice.
                                Focus on: Reading, Listening, Writing, Speaking skills, vocabulary, grammar, exam strategies, and band score requirements.
                                If asked about topics outside IELTS preparation, politely redirect to IELTS-related topics.";

        var conversationPrompt = new StringBuilder();
        
        if (history != null && history.Count > 0)
        {
            conversationPrompt.AppendLine("Previous conversation:");
            foreach (var msg in history.TakeLast(5)) // Keep last 5 messages for context
            {
                conversationPrompt.AppendLine($"{msg.Role}: {msg.Content}");
            }
            conversationPrompt.AppendLine();
        }
        
        conversationPrompt.AppendLine($"Student: {message}");
        conversationPrompt.AppendLine("IELTS Buddy:");

        return await GenerateContentAsync(conversationPrompt.ToString(), systemInstruction);
    }

    #region Helper Methods

    private static string CleanJsonResponse(string response)
    {
        // Remove markdown code blocks
        response = response.Trim();
        if (response.StartsWith("```json"))
            response = response[7..];
        else if (response.StartsWith("```"))
            response = response[3..];
        
        if (response.EndsWith("```"))
            response = response[..^3];
        
        return response.Trim();
    }

    private static string GetMockResponse(string query)
    {
        var topic = query.ToLower();
        
        if (topic.Contains("writing"))
            return "For IELTS Writing, focus on clear structure, coherent paragraphs, varied vocabulary, and accurate grammar. Practice writing at least 150 words for Task 1 and 250 words for Task 2.";
        
        if (topic.Contains("speaking"))
            return "For IELTS Speaking, practice speaking naturally about common topics. Focus on fluency, pronunciation, and using varied vocabulary. Record yourself and listen back to improve.";
        
        if (topic.Contains("reading"))
            return "For IELTS Reading, practice skimming and scanning techniques. Read English texts daily and time yourself. Focus on understanding main ideas and specific details.";
        
        if (topic.Contains("listening"))
            return "For IELTS Listening, practice with various English accents. Take notes while listening and predict answers before you hear them. Practice daily with podcasts or IELTS materials.";
        
        return "I'm your IELTS AI tutor! I can help you with Writing, Speaking, Reading, and Listening practice. Configure your Gemini API key in appsettings.json for full AI capabilities.";
    }

    private static WritingEvaluation GetMockWritingEval() => new()
    {
        OverallBand = 6.0f,
        TaskAchievement = 6.0f,
        CoherenceCohesion = 6.0f,
        LexicalResource = 6.0f,
        GrammaticalRange = 5.5f,
        Feedback = "Your essay addresses the task adequately. To improve: use more complex sentence structures, improve paragraph coherence, and expand your academic vocabulary.",
        Strengths = new List<string> 
        { 
            "Clear position stated in introduction", 
            "Relevant examples provided",
            "Logical paragraph organization"
        },
        Improvements = new List<string> 
        { 
            "Use more complex sentence structures", 
            "Improve coherence between paragraphs",
            "Include more topic-specific vocabulary"
        },
        GrammarErrors = new List<GrammarError>
        {
            new() { Text = "Example error", Correction = "Corrected version", Explanation = "Explanation of grammar rule" }
        }
    };

    private static SpeakingEvaluation GetMockSpeakingEval() => new()
    {
        OverallBand = 6.0f,
        Fluency = 6.0f,
        Pronunciation = 5.5f,
        LexicalResource = 6.0f,
        GrammaticalRange = 6.0f,
        Feedback = "Good fluency with some hesitation. Work on pronunciation of complex words and reduce filler words like 'um' and 'uh'.",
        Strengths = new List<string> 
        { 
            "Good topic development", 
            "Natural speech rhythm",
            "Appropriate use of linking words"
        },
        Improvements = new List<string> 
        { 
            "Reduce filler words", 
            "Practice stress patterns",
            "Use more idiomatic expressions"
        }
    };

    #endregion
}
