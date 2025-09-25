using FIAPCloudGames.Domain.Enumerators;

namespace FIAPCloudGames.Application.Requests
{
    public class UpdateGameRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public GameCategory? Category { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
