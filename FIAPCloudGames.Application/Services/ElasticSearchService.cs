using FIAPCloudGames.Domain.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
namespace FIAPCloudGames.Application.Services
{
    public class ElasticSearchService<T> : IElasticSearchService<T> where T : class, IEntity
    {
        private readonly HttpClient _httpClient;
        private readonly string _indexName;

        public ElasticSearchService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("Elasticsearch");
            // Define o nome do índice com base no nome do tipo T em minúsculas (ex: "game", "user")
            _indexName = (typeof(T).Name.ToLower()).Replace("viewmodel", "") + "s";
        }
        public async Task IndexDocumentAsync(T document)
        {
            // O Elasticsearch usa o ID do documento na URL para criar ou atualizar (upsert)
            var documentId = document.GetType().GetProperty("Id")?.GetValue(document)?.ToString();
            if (string.IsNullOrEmpty(documentId))
            {
                throw new ArgumentException("A entidade precisa ter uma propriedade 'Id'.");
            }

            var json = JsonConvert.SerializeObject(document);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // A rota é /<index>/_doc/<id>
            var response = await _httpClient.PutAsync($"/{_indexName}/_doc/{documentId}", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task<T?> GetDocumentAsync(string id)
        {
            // A rota é /<index>/_doc/<id>
            var response = await _httpClient.GetAsync($"/{_indexName}/_doc/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // Se o documento não for encontrado (404), retorna nulo.
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                // Para outros erros, lança uma exceção.
                response.EnsureSuccessStatusCode();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(jsonResponse);

            // O documento real está dentro do campo "_source"
            var source = jObject["_source"]?.ToString();

            return source != null ? JsonConvert.DeserializeObject<T>(source) : null;
        }
        public async Task DeleteDocumentAsync(string id)
        {
            // A rota é /<index>/_doc/<id>
            var response = await _httpClient.DeleteAsync($"/{_indexName}/_doc/{id}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<ICollection<T>> SearchAsync(string query)
        {
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // A rota de busca é /<index>/_search
            var response = await _httpClient.PostAsync($"/{_indexName}/_search", content);

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<T>().ToList();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(jsonResponse);

            // Os resultados estão em "hits.hits" e o documento em si em "_source"
            var hits = jObject["hits"]?["hits"]?.Select(h => h["_source"]?.ToString()).ToList();

            var results = new List<T>();
            if (hits != null)
            {
                foreach (var hit in hits)
                {
                    if (!string.IsNullOrEmpty(hit))
                        results.Add(JsonConvert.DeserializeObject<T>(hit));
                }
            }

            return results;
        }
        public async Task<string> AggregateAsync(string query)
        {
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // A rota de busca também é usada para agregações
            var response = await _httpClient.PostAsync($"/{_indexName}/_search", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(jsonResponse);

            // O resultado da agregação está no campo "aggregations"
            return jObject["aggregations"]?.ToString() ?? "{}";
        }
    }
}
