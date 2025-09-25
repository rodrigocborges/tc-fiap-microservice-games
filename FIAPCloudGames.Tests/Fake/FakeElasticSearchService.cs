using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Interfaces;

namespace FIAPCloudGames.Tests.Fake
{
    public class FakeElasticSearchService : IElasticSearchService<Game>
    {
        private readonly List<Game> _games = new List<Game>();

        public FakeElasticSearchService()
        {
            // Pré-carrega um jogo para os testes de duplicidade
            _games.Add(new Game("Minecraft", "Um jogo sandbox", 79.90M, Domain.Enumerators.GameCategory.Simulation, new DateTime(2009, 05, 17)));
        }

        public Task IndexDocumentAsync(Game document)
        {
            var existingGame = _games.FirstOrDefault(g => g.Id == document.Id);
            if (existingGame != null)
            {
                _games.Remove(existingGame);
            }
            _games.Add(document);
            return Task.CompletedTask;
        }

        public Task<Game?> GetDocumentAsync(string id)
        {
            var game = _games.FirstOrDefault(g => g.Id.ToString() == id);
            return Task.FromResult(game);
        }

        public Task DeleteDocumentAsync(string id)
        {
            var game = _games.FirstOrDefault(g => g.Id.ToString() == id);
            if (game != null)
            {
                _games.Remove(game);
            }
            return Task.CompletedTask;
        }

        public Task<ICollection<Game>> SearchAsync(string query)
        {
            // Simula a busca por nome para o teste de duplicidade
            if (query.Contains("name.keyword"))
            {
                // Extrai o nome da query de forma simples
                var nameToFind = query.Split(new[] { @"""query"": """ }, StringSplitOptions.None)[1].Split('"')[0];
                var results = _games.Where(g => g.Name.Equals(nameToFind, StringComparison.OrdinalIgnoreCase)).ToList();
                return Task.FromResult<ICollection<Game>>(results);
            }

            // Simula o FindAll
            if (query.Contains("match_all"))
            {
                return Task.FromResult<ICollection<Game>>(_games.ToList());
            }

            return Task.FromResult<ICollection<Game>>(new List<Game>());
        }

        public Task<string> AggregateAsync(string query)
        {
            // Retorna um JSON vazio, pois não é o foco dos testes unitários do GameService.
            return Task.FromResult("{}");
        }
    }
}