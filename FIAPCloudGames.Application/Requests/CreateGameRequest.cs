using FIAPCloudGames.Domain.Enumerators;

namespace FIAPCloudGames.Application.Requests
{
    public class CreateGameRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required GameCategory Category { get; set; }
        public required DateTime ReleaseDate { get; set; }
    }
}
