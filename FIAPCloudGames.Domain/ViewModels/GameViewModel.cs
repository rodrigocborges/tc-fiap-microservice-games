using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Enumerators;
using FIAPCloudGames.Domain.Interfaces;

namespace FIAPCloudGames.Domain.ViewModels
{
    public class GameViewModel : IEntity
    {
        public Guid Id { get;  set; }
        public string Name { get;  set; }
        public string? Description { get;  set; }
        public decimal Price { get; set; }
        public GameCategory Category { get;  set; }
        public DateTime ReleaseDate { get;  set; }
        public DateTime LastUpdate { get;  set; }

        public GameViewModel()
        {
            if(Id == Guid.Empty)
                Id = Guid.NewGuid();
        }

        public Game ToEntity()
        {
            return new Game(Name, Description, Price, Category, ReleaseDate);
        }
    }
}
