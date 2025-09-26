using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Interfaces;
using FIAPCloudGames.Domain.ViewModels;

namespace FIAPCloudGames.Tests.Fake
{
    public class FakeElasticSearchService : IElasticSearchService<GameViewModel>
    {
        private readonly List<GameViewModel> _games = new List<GameViewModel>();

        public FakeElasticSearchService()
        {
            // Pré-carrega um jogo para os testes de duplicidade
            _games.Add(new GameViewModel { Name = "Minecraft", Description = "Um jogo sandbox", Price = 79.90M, Category = Domain.Enumerators.GameCategory.Simulation, ReleaseDate = new DateTime(2009, 05, 17) });
        }

        public Task IndexDocumentAsync(GameViewModel document)
        {
            var existingGame = _games.FirstOrDefault(g => g.Id == document.Id);
            if (existingGame != null)
            {
                _games.Remove(existingGame);
            }
            _games.Add(document);
            return Task.CompletedTask;
        }

        public Task<GameViewModel?> GetDocumentAsync(string id)
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

        public Task<ICollection<GameViewModel>> SearchAsync(string query)
        {
            // Simula a busca por nome para o teste de duplicidade
            if (query.Contains("name.keyword"))
            {
                // Extrai o nome da query de forma simples
                var nameToFind = query.Split(new[] { @"""query"": """ }, StringSplitOptions.None)[1].Split('"')[0];
                var results = _games.Where(g => g.Name.Equals(nameToFind, StringComparison.OrdinalIgnoreCase)).ToList();
                return Task.FromResult<ICollection<GameViewModel>>(results);
            }

            // Simula o FindAll
            if (query.Contains("match_all"))
            {
                return Task.FromResult<ICollection<GameViewModel>>(_games.ToList());
            }

            return Task.FromResult<ICollection<GameViewModel>>(new List<GameViewModel>());
        }

        public Task<string> AggregateAsync(string query)
        {
            // Retorna um JSON vazio, pois não é o foco dos testes unitários do GameService.
            return Task.FromResult("{}");
        }
    }
}