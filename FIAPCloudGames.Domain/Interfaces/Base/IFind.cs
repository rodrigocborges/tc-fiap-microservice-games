namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IFind<T> where T : IEntity
    {
        Task<T?> Find(Guid id);
    }
}
