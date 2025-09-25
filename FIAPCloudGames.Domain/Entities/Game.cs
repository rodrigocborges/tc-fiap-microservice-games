using FIAPCloudGames.Domain.Enumerators;
using FIAPCloudGames.Domain.Interfaces;
using FIAPCloudGames.Domain.ValueObjects;
using FIAPCloudGames.Domain.ViewModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIAPCloudGames.Domain.Entities
{
    public class Game : IEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public Price Price { get; set; }
        public GameCategory Category { get; private set; }
        public DateTime ReleaseDate { get; private set; }
        public DateTime LastUpdate { get; private set; }
        private Game() { }

        public Game(string name, string? description, decimal price, GameCategory category, DateTime releaseDate)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Price = new Price(price);
            Category = category;
            ReleaseDate = releaseDate;
            LastUpdate = DateTime.UtcNow;
        }

        public void Update(string? name, string? description, decimal? price, GameCategory? category, DateTime? releaseDate)
        {
            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
                hasChanges = true;
            }

            if(!string.IsNullOrWhiteSpace(description))
            {
                Description = description;
                hasChanges = true;
            }

            if (price != null)
            {
                Price = new Price(price.Value);
                hasChanges = true;
            }

            if (category != null)
            {
                Category = category.Value;
                hasChanges = true;
            }

            if (releaseDate != null)
            {
                ReleaseDate = releaseDate.Value;
                hasChanges = true;
            }

            if (hasChanges)
                LastUpdate = DateTime.UtcNow;
        }
    
        public GameViewModel ToViewModel()
        {
            return new GameViewModel {
                Id = Id,
                Name = Name,
                Description = Description,
                Price = Price.Value,
                Category = Category,
                ReleaseDate = ReleaseDate,
                LastUpdate = LastUpdate
            };
        }
    }
}
