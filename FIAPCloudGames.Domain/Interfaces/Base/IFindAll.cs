namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IFindAll<T> where T : IEntity
    {
        Task<ICollection<T>?> FindAll(int skip = 0, int take = 10);
    }
}
