using FIAPCloudGames.Application.Requests;
using FIAPCloudGames.Application.Services;
using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Enumerators;
using FIAPCloudGames.Tests.Fake;

namespace FIAPCloudGames.Tests
{
    public class GameServiceTest
    {
        [Fact]
        public async Task CreateGame_WithValidData_ShouldAddGameAndReturnId()
        {
            // Arrange
            var fakeElasticService = new FakeElasticSearchService();
            var service = new GameService(fakeElasticService);

            var request = new CreateGameRequest
            {
                Name = "Call of Duty: Black Ops",
                Description = "Um jogo de tiro...",
                Price = 129.90M,
                Category = GameCategory.FPS,
                ReleaseDate = new DateTime(2010, 11, 09)
            };
            var newGame = new Game(request.Name, request.Description, request.Price, request.Category, request.ReleaseDate);

            // Act
            var resultId = await service.Create(newGame);

            // Assert
            Assert.Equal(newGame.Id, resultId); // O ID retornado deve ser o mesmo do objeto criado
            var createdGame = await fakeElasticService.GetDocumentAsync(resultId.ToString());
            Assert.NotNull(createdGame);
            Assert.Equal(request.Name, createdGame!.Name);
        }

        [Fact]
        public async Task CreateGame_WithDuplicateName_ShouldThrowException()
        {
            // Arrange
            var fakeElasticService = new FakeElasticSearchService(); // Já contém "Minecraft"
            var service = new GameService(fakeElasticService);

            var request = new CreateGameRequest
            {
                Name = "Minecraft", // Nome duplicado
                Description = "Um jogo sandbox e FPS.",
                Price = 79.90M,
                Category = GameCategory.FPS,
                ReleaseDate = new DateTime(2009, 05, 17)
            };
            var newGame = new Game(request.Name, request.Description, request.Price, request.Category, request.ReleaseDate);


            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.Create(newGame);
            });
        }
    }
}