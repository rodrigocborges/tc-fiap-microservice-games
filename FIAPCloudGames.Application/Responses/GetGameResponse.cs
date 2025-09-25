using FIAPCloudGames.Domain.Enumerators;

namespace FIAPCloudGames.Application.Responses
{
    public class GetGameResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryDescription { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
