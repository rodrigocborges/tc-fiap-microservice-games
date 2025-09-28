using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Enumerators;
using FIAPCloudGames.Domain.ViewModels;

namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IGameService : ICreate<GameViewModel>, IUpdate<GameViewModel>, IFind<GameViewModel>, 
        IFindAll<GameViewModel>, IDelete<GameViewModel>
    {
        Task<GameViewModel?> FindByName(string name);
        Task<string> GetCategoryMetrics();
        Task<ICollection<GameViewModel>> AdvancedSearch(int skip, int take, string? term, GameCategory? category, decimal? minPrice, decimal? maxPrice);
    }
}
