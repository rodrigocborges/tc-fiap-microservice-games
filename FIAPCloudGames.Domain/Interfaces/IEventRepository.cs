using FIAPCloudGames.Domain.Entities;
namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task Save(DomainEvent domainEvent);
        Task<IEnumerable<DomainEvent>> GetAll(int skip, int take);
        Task<IEnumerable<DomainEvent>> GetAllByAggregateId(Guid aggregateId);
    }
}
