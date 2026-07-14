using System.Text;
using System.Text.Json;

namespace LegalDocs.VectorSearch.Core
{
    public class LegalChunkRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }

    public class VectorMemoryManager
    {
        private readonly HttpClient _httpClient;
        private readonly List<LegalChunkRecord> _vectorCollection;
        private readonly string _ollamaEndpoint = "http://localhost:11434/api/embeddings";

        public VectorMemoryManager()
        {
            _httpClient = new HttpClient();
            _vectorCollection = new List<LegalChunkRecord>();
        }

        private async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var requestBody = new
            {
                model = "nomic-embed-text",
                prompt = text
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_ollamaEndpoint, content);

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            var embeddingArray = doc.RootElement.GetProperty("embedding").EnumerateArray();

            return embeddingArray.Select(x => x.GetSingle()).ToArray();
        }

        public async Task IngestTextAsync(string fullText)
        {
            Console.WriteLine("[Vector DB] Initializing custom in-memory collection...");

            var paragraphs = fullText.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            foreach (var paragraph in paragraphs)
            {
                string cleanText = paragraph.Trim();
                if (cleanText.Length < 15) continue;

                var embedding = await GenerateEmbeddingAsync(cleanText);

                _vectorCollection.Add(new LegalChunkRecord
                {
                    Text = cleanText,
                    Embedding = embedding
                });
                count++;
            }

            Console.WriteLine($"[Vector DB] Successfully embedded and stored {count} document chunks.");
        }

        public async Task<string> SearchRelevantContextAsync(string query)
        {
            Console.WriteLine("[Vector DB] Searching for relevant legal context...");
            var queryEmbedding = await GenerateEmbeddingAsync(query);

            var topResults = _vectorCollection
                .Select(record => new
                {
                    Record = record,
                    Similarity = CalculateCosineSimilarity(queryEmbedding, record.Embedding)
                })
                .OrderByDescending(x => x.Similarity)
                .Take(3) 
                .ToList();

            var contextBuilder = new StringBuilder();
            foreach (var result in topResults)
            {
                contextBuilder.AppendLine(result.Record.Text);
                contextBuilder.AppendLine("---");
            }

            return contextBuilder.ToString();
        }

        private float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            float dotProduct = 0;
            float magnitudeA = 0;
            float magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0) return 0;

            return dotProduct / (float)(Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}