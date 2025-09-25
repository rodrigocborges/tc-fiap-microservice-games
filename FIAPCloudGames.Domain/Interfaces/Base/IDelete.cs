namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IDelete<T> where T : IEntity
    {
        Task Delete(Guid id);
    }
}
