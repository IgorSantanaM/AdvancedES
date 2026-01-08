using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Domain.Tests
{
    public class TestStore : IEventStore
    {
        public List<StoredEvent> previousEvents = [];

        public List<StoredEvent> newEvents = [];
        public void AppendEvent(StoredEvent @event)
        {
            newEvents.Add(@event);
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
        {
            return previousEvents
                .Where(e => e.AggregateId == aggregateId)
                .ToList();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
