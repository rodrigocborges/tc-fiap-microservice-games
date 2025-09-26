using FIAPCloudGames.Domain.Entities;
using FIAPCloudGames.Domain.Interfaces;
using LiteDB;

namespace FIAPCloudGames.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ILiteDatabase _db;
        private const string _collectionName = "domain_events";

        public EventRepository(ILiteDatabase db)
        {
            _db = db;
        }

        public Task<IEnumerable<DomainEvent>> GetAll(int skip, int take)
        {
            var events = _db.GetCollection<DomainEvent>(_collectionName);
            var results = events.FindAll()?.Skip(skip)?.Take(take);
            return Task.FromResult(results ?? new List<DomainEvent>());
        }

        public Task<IEnumerable<DomainEvent>> GetAllByAggregateId(Guid aggregateId)
        {
            var events = _db.GetCollection<DomainEvent>(_collectionName);
            events.EnsureIndex(e => e.AggregateId);
            var results = events.Find(e => e.AggregateId == aggregateId);
            return Task.FromResult(results ?? new List<DomainEvent>());
        }

        public Task Save(DomainEvent domainEvent)
        {
            var events = _db.GetCollection<DomainEvent>(_collectionName);
            events.Insert(domainEvent);
            return Task.CompletedTask;
        }
    }
}
