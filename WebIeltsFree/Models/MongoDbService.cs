using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebIeltsFree.Models
{
    /// MongoDB service for storing analytics, AI logs, and rich media data
    public interface IMongoDbService
    {
        Task LogUserBehaviorAsync(int userId, string action, object metadata);
        Task SaveLearningPatternAsync(int userId, object patterns);
        Task SaveAiPredictionAsync(int userId, string predictionType, object input, object output);
        Task SaveSpeakingSessionDetailAsync(int userId, int sessionId, object analysisData);
        Task SaveWritingAnalysisDetailAsync(int userId, int submissionId, object analysisData);
        Task SaveAiConversationAsync(int userId, string conversationId, List<object> messages);
        Task<dynamic> GetUserBehaviorLogsAsync(int userId, DateTime fromDate);
        Task<dynamic> GetLearningPatternAsync(int userId);
        IMongoCollection<BsonDocument> GetCollection(string collectionName);
    }

    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbService> _logger;

        public MongoDbService(IConfiguration configuration, ILogger<MongoDbService> logger)
        {
            _logger = logger;
            
            // Resolve environment variable placeholders
            var rawConnString = configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
            var connectionString = EnvHelper.ResolveEnvVars(rawConnString);
            
            var rawDbName = configuration["MongoDB:DatabaseName"] ?? "IELTSDb";
            var databaseName = EnvHelper.ResolveEnvVars(rawDbName);

            try
            {
                _mongoClient = new MongoClient(connectionString);
                _database = _mongoClient.GetDatabase(databaseName);
                
                _logger.LogInformation($" MongoDB connected: {databaseName}");
                InitializeCollections();
            }
            catch (Exception ex)
            {
                _logger.LogError($" MongoDB connection failed: {ex.Message}");
                throw;
            }
        }

        private void InitializeCollections()
        {
            try
            {
                // User behavior logs - auto-delete after 90 days
                var behaviorCollection = _database.GetCollection<BsonDocument>("user_behavior_logs");
                var behaviorIndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys.Ascending("timestamp"),
                    new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(90) }
                );
                behaviorCollection.Indexes.CreateOne(behaviorIndexModel);

                var patternsCollection = _database.GetCollection<BsonDocument>("learning_patterns");
                patternsCollection.Indexes.CreateOne(
                    new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("userId")
                    )
                );

                // AI predictions - auto-delete after 30 days
                var predictionsCollection = _database.GetCollection<BsonDocument>("ai_predictions");
                var predictionsIndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys.Ascending("createdAt"),
                    new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(30) }
                );
                predictionsCollection.Indexes.CreateOne(predictionsIndexModel);

                // Speaking session details
                var speakingCollection = _database.GetCollection<BsonDocument>("speaking_sessions_detail");
                speakingCollection.Indexes.CreateOne(
                    new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("userId")
                    )
                );

                // Writing analysis details
                var writingCollection = _database.GetCollection<BsonDocument>("writing_analyses_detail");
                writingCollection.Indexes.CreateOne(
                    new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("userId")
                    )
                );

                // AI conversations
                var conversationsCollection = _database.GetCollection<BsonDocument>("ai_conversations");
                conversationsCollection.Indexes.CreateOne(
                    new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("userId")
                    )
                );

                _logger.LogInformation(" MongoDB collections initialized with TTL indexes");
            }
            catch (Exception ex)
            {
                _logger.LogError($" Error initializing MongoDB collections: {ex.Message}");
            }
        }

        public async Task LogUserBehaviorAsync(int userId, string action, object metadata)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("user_behavior_logs");
                var document = new BsonDocument
                {
                    { "userId", userId },
                    { "action", action },
                    { "metadata", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(metadata ?? new {})) },
                    { "timestamp", DateTime.UtcNow },
                    { "_id", ObjectId.GenerateNewId() }
                };

                await collection.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging user behavior: {ex.Message}");
            }
        }

        /// Save user learning patterns (study times, preferences, pace, consistency)
        public async Task SaveLearningPatternAsync(int userId, object patterns)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("learning_patterns");
                var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
                
                var document = new BsonDocument
                {
                    { "userId", userId },
                    { "patterns", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(patterns)) },
                    { "analyzedAt", DateTime.UtcNow }
                };

                var options = new ReplaceOptions { IsUpsert = true };
                await collection.ReplaceOneAsync(filter, document, options);

                _logger.LogInformation($" Learning pattern saved for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving learning pattern: {ex.Message}");
            }
        }

        /// Save AI predictions (band scores, skill improvements, exam readiness)
        public async Task SaveAiPredictionAsync(int userId, string predictionType, object input, object output)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("ai_predictions");
                var document = new BsonDocument
                {
                    { "userId", userId },
                    { "predictionType", predictionType },
                    { "input", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(input ?? new {})) },
                    { "output", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(output ?? new {})) },
                    { "model", "gemini-2.5-flash" },
                    { "confidence", 0.85 },
                    { "createdAt", DateTime.UtcNow },
                    { "_id", ObjectId.GenerateNewId() }
                };

                await collection.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving AI prediction: {ex.Message}");
            }
        }

        // Save detailed speaking session analysis from MongoDB (audio URL, transcript, AI feedback)
        public async Task SaveSpeakingSessionDetailAsync(int userId, int sessionId, object analysisData)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("speaking_sessions_detail");
                var document = new BsonDocument
                {
                    { "userId", userId },
                    { "mysqlSessionId", sessionId },
                    { "aiAnalysis", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(analysisData)) },
                    { "duration", 0 },
                    { "createdAt", DateTime.UtcNow },
                    { "_id", ObjectId.GenerateNewId() }
                };

                await collection.InsertOneAsync(document);

                _logger.LogInformation($" Speaking session detail saved for session {sessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving speaking session detail: {ex.Message}");
            }
        }

        /// <summary>
        /// Save detailed writing analysis (paragraph breakdown, grammar errors, vocabulary analysis)
        /// </summary>
        public async Task SaveWritingAnalysisDetailAsync(int userId, int submissionId, object analysisData)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("writing_analyses_detail");
                var document = new BsonDocument
                {
                    { "userId", userId },
                    { "mysqlSubmissionId", submissionId },
                    { "detailedAnalysis", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(analysisData)) },
                    { "_id", ObjectId.GenerateNewId() }
                };

                await collection.InsertOneAsync(document);

                _logger.LogInformation($" Writing analysis detail saved for submission {submissionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving writing analysis detail: {ex.Message}");
            }
        }

        /// <summary>
        /// Save AI conversation history for RAG and learning context
        /// </summary>
        public async Task SaveAiConversationAsync(int userId, string conversationId, List<object> messages)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("ai_conversations");
                var filter = Builders<BsonDocument>.Filter.Eq("conversationId", conversationId);

                var msgArray = new BsonArray();
                if (messages != null)
                {
                    foreach (var msg in messages)
                    {
                        msgArray.Add(BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(msg)));
                    }
                }

                var document = new BsonDocument
                {
                    { "userId", userId },
                    { "conversationId", conversationId },
                    { "messages", msgArray },
                    { "createdAt", DateTime.UtcNow },
                    { "updatedAt", DateTime.UtcNow }
                };

                var options = new ReplaceOptions { IsUpsert = true };
                await collection.ReplaceOneAsync(filter, document, options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving AI conversation: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user behavior logs for analytics dashboard
        /// </summary>
        public async Task<dynamic> GetUserBehaviorLogsAsync(int userId, DateTime fromDate)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("user_behavior_logs");
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("userId", userId),
                    Builders<BsonDocument>.Filter.Gte("timestamp", fromDate)
                );

                var logs = await collection.Find(filter)
                    .Sort(Builders<BsonDocument>.Sort.Descending("timestamp"))
                    .Limit(1000)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving behavior logs: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get user learning pattern for personalized recommendations
        /// </summary>
        public async Task<dynamic> GetLearningPatternAsync(int userId)
        {
            try
            {
                var collection = _database.GetCollection<BsonDocument>("learning_patterns");
                var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);

                var pattern = await collection.Find(filter).FirstOrDefaultAsync();
                return pattern;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving learning pattern: {ex.Message}");
                return null;
            }
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _database.GetCollection<BsonDocument>(collectionName);
        }
    }
}
