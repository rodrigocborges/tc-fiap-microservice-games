namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IUpdate<T> where T : IEntity
    {
        Task Update(T model);
    }
}
