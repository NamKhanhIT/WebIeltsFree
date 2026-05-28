using System.Text;
using System.Text.Json;
using WebIeltsFree.Models;

namespace WebIeltsFree.Services;

#region Interfaces

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}

public interface IVectorSearchService
{
    Task<List<SearchResult>> SearchAsync(string query, int topK = 5);
    Task IndexDocumentAsync(string id, string content, Dictionary<string, object>? metadata = null);
    Task DeleteDocumentAsync(string id);
}

public interface ISpacedRepetitionService
{
    Task<List<ReviewItem>> GetDueItemsAsync(int userId);
    Task UpdateItemAsync(int userId, int itemId, int quality);
    Task<DateTime> CalculateNextReviewAsync(ReviewItem item, int quality);
}

#endregion

#region DTOs for AI Services

public class WritingEvaluation
{
    private float _overallBand;
    public float OverallBand 
    { 
        get => _overallBand; 
        set => _overallBand = (float)Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2; 
    }
    public float TaskAchievement { get; set; }
    public float CoherenceCohesion { get; set; }
    public float LexicalResource { get; set; }
    public float GrammaticalRange { get; set; }
    public string Feedback { get; set; } = "";
    public List<string> Strengths { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
    public List<GrammarError> GrammarErrors { get; set; } = new();
}

public class GrammarError
{
    public string Text { get; set; } = "";
    public string Correction { get; set; } = "";
    public string Explanation { get; set; } = "";
    public int Position { get; set; }
}

public class SpeakingEvaluation
{
    private float _overallBand;
    public float OverallBand 
    { 
        get => _overallBand; 
        set => _overallBand = (float)Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2; 
    }
    public float Fluency { get; set; }
    public float Pronunciation { get; set; }
    public float LexicalResource { get; set; }
    public float GrammaticalRange { get; set; }
    public string Feedback { get; set; } = "";
    public List<string> Strengths { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
}

public class AiUserProfile
{
    public int UserId { get; set; }
    public int CurrentBand { get; set; }
    public int TargetBand { get; set; }
    public Dictionary<string, float> SkillScores { get; set; } = new();
    public List<string> WeakAreas { get; set; } = new();
    public List<string> CompletedLessons { get; set; } = new();
    public int StreakDays { get; set; }
}

public class SearchResult
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
    public float Score { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ReviewItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ItemType { get; set; } = "";
    public int ItemId { get; set; }
    public DateTime NextReview { get; set; }
    public int Interval { get; set; } = 1;
    public float EaseFactor { get; set; } = 2.5f;
    public int Repetitions { get; set; }
}

#endregion

#region Cache Service(In-Memory, replace with Redis in production)

public class CacheService : ICacheService
{
    private readonly Dictionary<string, (object Value, DateTime? Expiry)> _cache = new();
    private readonly ILogger<CacheService> _logger;

    public CacheService(ILogger<CacheService> logger)
    {
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiry == null || entry.Expiry > DateTime.UtcNow)
            {
                return Task.FromResult((T?)entry.Value);
            }
            _cache.Remove(key);
        }
        return Task.FromResult(default(T?));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var expiryTime = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : (DateTime?)null;
        _cache[key] = (value!, expiryTime);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiry == null || entry.Expiry > DateTime.UtcNow)
                return Task.FromResult(true);
            _cache.Remove(key);
        }
        return Task.FromResult(false);
    }
}

#endregion

#region Vector Search Service (Simple keyword-based, replace with Pinecone in production)

public class VectorSearchService : IVectorSearchService
{
    private readonly ILogger<VectorSearchService> _logger;
    private readonly Dictionary<string, (string Content, Dictionary<string, object> Metadata)> _documents = new();
    
    private readonly string? _pineconeApiKey;
    private readonly string? _pineconeIndexName;
    private readonly string? _geminiApiKey;
    private readonly HttpClient _httpClient;
    private string? _pineconeHost;

    public VectorSearchService(ILogger<VectorSearchService> logger, IConfiguration config)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        
        var pineconeKey = EnvHelper.ResolveEnvVars(config["Pinecone:ApiKey"]);
        _pineconeApiKey = string.IsNullOrWhiteSpace(pineconeKey) || pineconeKey.Contains("${") ? null : pineconeKey;
        
        _pineconeIndexName = EnvHelper.ResolveEnvVars(config["Pinecone:IndexName"]) ?? "ielts_knowledges";
        
        var geminiKey = EnvHelper.ResolveEnvVars(config["Gemini:ApiKey"]);
        _geminiApiKey = string.IsNullOrWhiteSpace(geminiKey) || geminiKey.Contains("${") ? null : geminiKey;
        
        InitializeKnowledgeBase();
    }

    private void InitializeKnowledgeBase()
    {
        var knowledge = new Dictionary<string, string>
        {
            ["grammar-past-perfect"] = "Past Perfect tense (had + past participle) is used for an action completed before another past action. Example: 'I had finished my work before he arrived.'",
            ["grammar-conditionals"] = "There are 4 types of conditionals: Zero (general truth), First (real possibility), Second (unreal present), Third (unreal past). Example Second: 'If I had more time, I would study more.'",
            ["grammar-articles"] = "Articles (a, an, the): Use 'a' before consonant sounds, 'an' before vowel sounds, 'the' for specific nouns. Common IELTS errors include missing articles before singular countable nouns.",
            ["writing-task1"] = "IELTS Writing Task 1 requires describing visual data (charts, graphs, maps, processes) in 150+ words. Include overview, key features, and comparisons. Avoid personal opinions.",
            ["writing-task2"] = "IELTS Writing Task 2 is an essay of 250+ words. Types include: Opinion, Discussion, Problem/Solution, Two-part question. Structure: Introduction, Body paragraphs, Conclusion.",
            ["writing-coherence"] = "Coherence in IELTS Writing means logical flow between ideas. Use linking words (However, Furthermore, In contrast), clear paragraph structure, and reference words (this, these, such).",
            ["speaking-part1"] = "Speaking Part 1 lasts 4-5 minutes. Examiner asks about familiar topics (home, work, studies, hobbies). Give extended answers with examples, not just yes/no.",
            ["speaking-part2"] = "Speaking Part 2 (Cue Card): 1 minute to prepare, 2 minutes to speak. Follow all bullet points on the card. Use past/present tense as needed.",
            ["speaking-part3"] = "Speaking Part 3 (Discussion): 4-5 minutes of abstract questions related to Part 2 topic. Show critical thinking, give opinions with reasons.",
            ["speaking-fluency"] = "Fluency means speaking at natural pace with few hesitations. Practice techniques: shadow native speakers, use filler phrases naturally (Well, Actually, To be honest).",
            ["reading-tfng"] = "True/False/Not Given: TRUE = exactly matches the passage, FALSE = contradicts the passage, NOT GIVEN = information not in passage. Don't use outside knowledge.",
            ["reading-matching"] = "Matching Headings: Read paragraphs for main idea, eliminate obvious wrong headings first. Watch for distractors that mention same words but different meaning.",
            ["reading-speed"] = "Improve reading speed: Practice skimming (main ideas) and scanning (specific info). Don't read every word. Time yourself with practice tests.",
            ["listening-tips"] = "IELTS Listening tips: Read questions before audio plays, predict answer types, watch for synonyms, write answers immediately, check spelling and grammar.",
            ["listening-maps"] = "Map/Diagram labeling: Study the map orientation first, listen for direction words (opposite, adjacent, between), follow the speaker's description path.",
            ["band-descriptors"] = "IELTS bands 1-9: Band 5=Modest user, Band 6=Competent user, Band 7=Good user, Band 8=Very good user. Each 0.5 improvement requires significant skill development.",
            ["vocabulary-collocations"] = "Collocations are words that naturally go together. Example: 'make a decision' (not 'do a decision'), 'heavy rain' (not 'strong rain'). Learn these for natural expression.",
            ["test-format"] = "IELTS has 4 sections: Listening (30min), Reading (60min), Writing (60min), Speaking (11-14min). Total test time is about 2 hours 45 minutes."
        };

        foreach (var item in knowledge)
        {
            _documents[item.Key] = (item.Value, new Dictionary<string, object> { ["type"] = "knowledge" });
        }
    }

    private async Task<string?> GetPineconeHostAsync()
    {
        if (_pineconeHost != null) return _pineconeHost;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.pinecone.io/indexes/{_pineconeIndexName}");
        request.Headers.Add("Api-Key", _pineconeApiKey);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("host", out var hostElem))
            {
                _pineconeHost = hostElem.GetString();
                return _pineconeHost;
            }
        }
        else 
        {
             _logger.LogWarning($"Failed to get Pinecone host: {response.StatusCode}");
        }
        return null;
    }

    private async Task<float[]?> GetEmbeddingAsync(string text)
    {
        if (string.IsNullOrEmpty(_geminiApiKey))
            return null;

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-exp-03-07:embedContent?key={_geminiApiKey}";
        
        var requestBody = new
        {
            content = new
            {
                parts = new[] { new { text = text } }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("embedding", out var embeddingObj) && 
                embeddingObj.TryGetProperty("values", out var values))
            {
                return values.EnumerateArray().Select(x => x.GetSingle()).ToArray();
            }
        }

        _logger.LogWarning($"Failed to get embedding: {await response.Content.ReadAsStringAsync()}");
        return null;
    }

    public async Task<List<SearchResult>> SearchAsync(string query, int topK = 5)
    {
        if (string.IsNullOrEmpty(_pineconeApiKey))
        {
            return FallbackSearch(query, topK);
        }

        try
        {
            var host = await GetPineconeHostAsync();
            if (host == null)
            {
                _logger.LogWarning("Falling back to in-memory search because Pinecone host could not be determined.");
                return FallbackSearch(query, topK);
            }

            var vector = await GetEmbeddingAsync(query);
            if (vector == null)
            {
                _logger.LogWarning("Failed to generate embedding. Falling back to in-memory search.");
                return FallbackSearch(query, topK);
            }

            var requestBody = new
            {
                vector = vector,
                topK = topK,
                includeMetadata = true
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{host}/query");
            request.Headers.Add("Api-Key", _pineconeApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                
                var results = new List<SearchResult>();
                if (doc.RootElement.TryGetProperty("matches", out var matches))
                {
                    foreach (var match in matches.EnumerateArray())
                    {
                        var id = match.GetProperty("id").GetString() ?? "";
                        var score = match.TryGetProperty("score", out var s) ? s.GetSingle() : 0f;
                        
                        var metadata = new Dictionary<string, object>();
                        var contentStr = "";
                        if (match.TryGetProperty("metadata", out var meta))
                        {
                            foreach (var prop in meta.EnumerateObject())
                            {
                                if (prop.Name == "text")
                                    contentStr = prop.Value.GetString() ?? "";
                                else
                                    metadata[prop.Name] = prop.Value.ToString() ?? "";
                            }
                        }

                        // If content wasn't in metadata but exists in our local memory fallback
                        if (string.IsNullOrEmpty(contentStr) && _documents.TryGetValue(id, out var localDoc))
                        {
                            contentStr = localDoc.Content;
                        }

                        results.Add(new SearchResult
                        {
                            Id = id,
                            Content = contentStr,
                            Score = score,
                            Metadata = metadata
                        });
                    }
                }
                return results;
            }
            else
            {
                _logger.LogWarning($"Pinecone search failed: {await response.Content.ReadAsStringAsync()}. Falling back to in-memory.");
                return FallbackSearch(query, topK);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during Pinecone search: {ex.Message}. Falling back.");
            return FallbackSearch(query, topK);
        }
    }

    private List<SearchResult> FallbackSearch(string query, int topK)
    {
        var queryWords = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return _documents
            .Select(d => new
            {
                Id = d.Key,
                Content = d.Value.Content,
                Metadata = d.Value.Metadata,
                Score = queryWords.Count(w => d.Value.Content.ToLower().Contains(w)) / (float)queryWords.Length
            })
            .Where(r => r.Score > 0.1f)
            .OrderByDescending(r => r.Score)
            .Take(topK)
            .Select(r => new SearchResult
            {
                Id = r.Id,
                Content = r.Content,
                Score = r.Score,
                Metadata = r.Metadata
            })
            .ToList();
    }

    public async Task IndexDocumentAsync(string id, string content, Dictionary<string, object>? metadata = null)
    {
        _documents[id] = (content, metadata ?? new Dictionary<string, object>());
        
        if (string.IsNullOrEmpty(_pineconeApiKey))
        {
            _logger.LogInformation("Indexed document in memory only: {Id}", id);
            return;
        }

        try
        {
            var host = await GetPineconeHostAsync();
            if (host == null) return;

            var vector = await GetEmbeddingAsync(content);
            if (vector == null) return;

            var meta = metadata ?? new Dictionary<string, object>();
            meta["text"] = content; 

            var requestBody = new
            {
                vectors = new[]
                {
                    new
                    {
                        id = id,
                        values = vector,
                        metadata = meta
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{host}/vectors/upsert");
            request.Headers.Add("Api-Key", _pineconeApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Indexed document in Pinecone: {Id}", id);
            }
            else
            {
                _logger.LogWarning($"Failed to index in Pinecone: {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error indexing in Pinecone: {ex.Message}");
        }
    }

    public async Task DeleteDocumentAsync(string id)
    {
        _documents.Remove(id);
        
        if (string.IsNullOrEmpty(_pineconeApiKey)) return;

        try
        {
            var host = await GetPineconeHostAsync();
            if (host == null) return;

            var requestBody = new
            {
                ids = new[] { id }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{host}/vectors/delete");
            request.Headers.Add("Api-Key", _pineconeApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to delete from Pinecone: {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting from Pinecone: {ex.Message}");
        }
    }
}

#endregion

#region Spaced Repetition Service (SM-2 Algorithm)

public class SpacedRepetitionService : ISpacedRepetitionService
{
    private readonly Dictionary<(int UserId, int ItemId), ReviewItem> _items = new();
    private readonly ILogger<SpacedRepetitionService> _logger;

    public SpacedRepetitionService(ILogger<SpacedRepetitionService> logger)
    {
        _logger = logger;
    }

    public Task<List<ReviewItem>> GetDueItemsAsync(int userId)
    {
        var now = DateTime.UtcNow;
        var dueItems = _items.Values
            .Where(i => i.UserId == userId && i.NextReview <= now)
            .OrderBy(i => i.NextReview)
            .Take(20)
            .ToList();

        return Task.FromResult(dueItems);
    }

    public async Task UpdateItemAsync(int userId, int itemId, int quality)
    {
        var key = (userId, itemId);
        if (!_items.TryGetValue(key, out var item))
        {
            item = new ReviewItem
            {
                Id = _items.Count + 1,
                UserId = userId,
                ItemId = itemId,
                ItemType = "vocabulary"
            };
        }

        item.NextReview = await CalculateNextReviewAsync(item, quality);
        _items[key] = item;
    }

    public Task<DateTime> CalculateNextReviewAsync(ReviewItem item, int quality)
    {
        // SM-2 Algorithm: quality 0-2 = fail, 3-5 = pass (3=hard, 4=good, 5=easy)
        
        if (quality < 3)
        {
            item.Repetitions = 0;
            item.Interval = 1;
        }
        else
        {
            if (item.Repetitions == 0)
                item.Interval = 1;
            else if (item.Repetitions == 1)
                item.Interval = 6;
            else
                item.Interval = (int)(item.Interval * item.EaseFactor);

            item.Repetitions++;
        }

        item.EaseFactor = Math.Max(1.3f, 
            item.EaseFactor + (0.1f - (5 - quality) * (0.08f + (5 - quality) * 0.02f)));

        return Task.FromResult(DateTime.UtcNow.AddDays(item.Interval));
    }
}

#endregion

#region RAG Chatbot Service

public interface IChatbotService
{
    Task<ChatResponse> GetResponseAsync(int userId, string message, List<ChatMessage>? history = null);
}

public class ChatMessage
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ChatResponse
{
    public string Message { get; set; } = "";
    public List<SearchResult> Sources { get; set; } = new();
    public List<string> SuggestedQuestions { get; set; } = new();
}

public class ChatbotService : IChatbotService
{
    private readonly IGeminiService _gemini;
    private readonly IVectorSearchService _vectorSearch;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(
        IGeminiService gemini, 
        IVectorSearchService vectorSearch,
        ILogger<ChatbotService> logger)
    {
        _gemini = gemini;
        _vectorSearch = vectorSearch;
        _logger = logger;
    }

    public async Task<ChatResponse> GetResponseAsync(int userId, string message, List<ChatMessage>? history = null)
    {
        var searchResults = await _vectorSearch.SearchAsync(message, topK: 3);
        var context = string.Join("\n\n", searchResults.Select(r => r.Content));

        string response;
        
        try
        {
            var chatHistory = history?.Select(h => new ChatMessage { Role = h.Role, Content = h.Content }).ToList();
            var enhancedMessage = !string.IsNullOrEmpty(context) 
                ? $"IELTS Knowledge Context:\n{context}\n\nStudent Question: {message}"
                : message;
            response = await _gemini.ChatAsync(enhancedMessage, chatHistory);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gemini API error, using fallback response");
            response = GetFallbackResponse(message, context);
        }

        var suggestions = GenerateSuggestions(message);

        return new ChatResponse
        {
            Message = response,
            Sources = searchResults,
            SuggestedQuestions = suggestions
        };
    }

    private string GetFallbackResponse(string message, string context)
    {
        if (!string.IsNullOrEmpty(context))
        {
            return $"Based on our IELTS knowledge base:\n\n{context}\n\nFor more detailed information, please configure your Gemini API key.";
        }
        return "I'm currently having trouble connecting to the AI service. Please try again later or configure your Gemini API key in appsettings.json.";
    }

    private List<string> GenerateSuggestions(string query)
    {
        var suggestions = new List<string>();
        var lowerQuery = query.ToLower();

        if (lowerQuery.Contains("writing"))
            suggestions.Add("How do I structure a Task 2 essay?");
        if (lowerQuery.Contains("speaking"))
            suggestions.Add("What are common Part 2 cue card topics?");
        if (lowerQuery.Contains("reading"))
            suggestions.Add("How do I improve my reading speed?");
        if (lowerQuery.Contains("listening"))
            suggestions.Add("What are the different section types in Listening?");
        if (lowerQuery.Contains("grammar"))
            suggestions.Add("What grammar is most important for Band 7?");
        if (lowerQuery.Contains("vocabulary"))
            suggestions.Add("How can I expand my vocabulary quickly?");

        if (!suggestions.Any())
        {
            suggestions.AddRange(new[]
            {
                "How can I improve my band score?",
                "What should I practice today?",
                "Explain the IELTS scoring system",
                "How do I prepare for the exam?",
                "What are the different sections of the exam?",
                "How do I structure a Task 2 essay?",
                "What are common Part 2 cue card topics?",
                "How do I improve my reading speed?",
                "What are the different section types in Listening?",
                "What grammar is most important for Band 7?",
                "How can I expand my vocabulary quickly?"
            });
        }

        return suggestions.Take(3).ToList();
    }
}

#endregion
