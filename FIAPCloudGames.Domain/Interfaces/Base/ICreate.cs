namespace FIAPCloudGames.Domain.Interfaces
{
    public interface ICreate<T> where T : IEntity
    {
        Task<Guid> Create(T model);
    }
}
