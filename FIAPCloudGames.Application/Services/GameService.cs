using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Enumerators;
using FIAPCloudGames.Domain.Interfaces;
using FIAPCloudGames.Domain.ViewModels;
using Newtonsoft.Json;

namespace FIAPCloudGames.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IElasticSearchService<GameViewModel> _elasticSearchService;
        private readonly IEventRepository _event;
        public GameService(IElasticSearchService<GameViewModel> elasticSearchService, IEventRepository @event)
        {
            _elasticSearchService = elasticSearchService;
            _event = @event;
        }

        public async Task<Guid> Create(GameViewModel model)
        {
            var gameFound = await FindByName(model.Name);
            if (gameFound != null)
                throw new InvalidOperationException("Já existe um game com esse nome!");
            
            await _elasticSearchService.IndexDocumentAsync(model);
            await _event.Save(new DomainEvent(model.Id, "GameCreated", JsonConvert.SerializeObject(model)));

            return model.Id;
        }

        public async Task Update(GameViewModel model)
        {
            // O método IndexDocumentAsync do Elasticsearch funciona como um "upsert":
            // se o documento com o ID já existe, ele é atualizado.
            await _elasticSearchService.IndexDocumentAsync(model);
            await _event.Save(new DomainEvent(model.Id, "GameUpdated", JsonConvert.SerializeObject(model)));
        }

        public async Task Delete(Guid id)
        {
            await _elasticSearchService.DeleteDocumentAsync(id.ToString());
            await _event.Save(new DomainEvent(id, "GameDeleted"));
        }
        public async Task<GameViewModel?> Find(Guid id)
            => await _elasticSearchService.GetDocumentAsync(id.ToString());

        public async Task<ICollection<GameViewModel>?> FindAll(int skip = 0, int take = 10)
        {
            // Monta uma query "match_all" para retornar todos os documentos, com paginação
            var query = $@"
            {{
              ""from"": {skip},
              ""size"": {take},
              ""query"": {{
                ""match_all"": {{}}
              }}
            }}";
            return await _elasticSearchService.SearchAsync(query);
        }

        public async Task<GameViewModel?> FindByName(string name)
        {
            // Monta uma query "match" para buscar um nome exato, ignorando case.
            // O campo ".keyword" é usado para correspondência exata.
            var query = $@"
            {{
              ""query"": {{
                ""match"": {{
                  ""name.keyword"": {{
                    ""query"": ""{name}"",
                    ""case_insensitive"": true
                  }}
                }}
              }}
            }}";

            var result = await _elasticSearchService.SearchAsync(query);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// Realiza uma busca avançada por múltiplos critérios.
        /// </summary>
        /// <param name="term">Um termo para buscar no nome ou descrição.</param>
        /// <param name="category">A categoria do jogo.</param>
        /// <param name="minPrice">Preço mínimo.</param>
        /// <param name="maxPrice">Preço máximo.</param>
        /// <returns>Uma coleção de jogos que atendem aos critérios.</returns>
        public async Task<ICollection<GameViewModel>> AdvancedSearch(int skip, int take, string? term, GameCategory? category, decimal? minPrice, decimal? maxPrice)
        {
            var mustClauses = new List<string>();

            // Cláusula para busca textual no nome e descrição
            if (!string.IsNullOrWhiteSpace(term))
            {
                mustClauses.Add($@"
                {{
                  ""multi_match"": {{
                    ""query"": ""{term}"",
                    ""fields"": [""Name"", ""Description""]
                  }}
                }}");
            }

            // Cláusula de filtro por categoria
            if (category.HasValue)
            {
                mustClauses.Add($@"
                {{
                  ""term"": {{
                    ""Category"": ""{category}""
                  }}
                }}");
            }

            // Cláusula de filtro por intervalo de preço (range)
            var rangeClauses = new List<string>();
            if (minPrice.HasValue)
            {
                rangeClauses.Add($"\"gte\": {minPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
            if (maxPrice.HasValue)
            {
                rangeClauses.Add($"\"lte\": {maxPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
            if (rangeClauses.Any())
            {
                mustClauses.Add($@"
                {{
                  ""range"": {{
                    ""Price"": {{
                      {string.Join(", ", rangeClauses)}
                    }}
                  }}
                }}");
            }

            var query = $@"
            {{
              ""from"": {skip},
              ""size"": {take},
              ""query"": {{
                ""bool"": {{
                  ""must"": [
                    {string.Join(", ", mustClauses)}
                  ]
                }}
              }}
            }}";

            return await _elasticSearchService.SearchAsync(query);
        }

        public async Task<string> GetCategoryMetrics()
        {
            // Esta query de agregação não retorna nenhum jogo ("size": 0),
            // apenas os resultados da agregação.
            // Ela agrupa os documentos pelo campo "category" e conta quantos jogos há em cada.
            var query = @"
            {
              ""size"": 0,
              ""aggs"": {
                ""games_by_category"": {
                  ""terms"": {
                    ""field"": ""Category"" 
                  }
                }
              }
            }";

            return await _elasticSearchService.AggregateAsync(query);
        }
    }
}