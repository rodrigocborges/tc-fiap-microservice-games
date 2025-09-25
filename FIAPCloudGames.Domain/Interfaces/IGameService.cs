using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.ViewModels;

namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IGameService : ICreate<GameViewModel>, IUpdate<GameViewModel>, IFind<GameViewModel>, 
        IFindAll<GameViewModel>, IDelete<GameViewModel>
    {
        Task<GameViewModel?> FindByName(string name);
        Task<string> GetCategoryMetrics();
    }
}
