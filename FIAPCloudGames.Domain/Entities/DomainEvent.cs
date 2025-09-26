using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAPCloudGames.Domain.Entities
{
    public class DomainEvent
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public DateTime Timestamp { get; set; }

        public DomainEvent() { }

        public DomainEvent(Guid aggregateId, string eventType, string eventData = "")
        {
            Id = Guid.NewGuid();
            AggregateId = aggregateId;
            EventType = eventType;
            EventData = eventData;
            Timestamp = DateTime.UtcNow;
        }
    }
}
