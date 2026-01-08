using BeerSender.Domain;
using Dapper;

namespace BeerSender.EventStore
{
    public class EventStore(INotificationService notificationService, EventStoreConnectionFactory dbConnection) : IEventStore
    {
        private List<StoredEvent> _newEvents = new();

        public void AppendEvent(StoredEvent @event)
        {
            _newEvents.Add(@event);
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
        {
            var query = """
                SELECT [AggregateId], [SequenceNumber], [TimeStamp]
                       ,[EventTypeName], [EventBody], [RowVersion]
                From dbo.[Events]
                WHERE [AggregateId] = @AggregateId
                ORDER BY [SequenceNumber]
                """;

            DynamicParameters dp = new DynamicParameters();

            dp.Add("AggregateId", aggregateId);

            using var connection = dbConnection.Create();

            IEnumerable<StoredEvent> storedEvents = connection.Query<DatabaseEvent>(query, dp)
                                                        .Select(e => e.ToStoredEvent());

            return storedEvents;
        }

        public void SaveChanges()
        {
            var insertCommand = """
                        INSERT INTO dbo.[Events]
                                    ([AggregateId], [SequenceNumber], [TimeStamp]
                                   , [EventTypeName], [EventBody])
                        VALUES 
                                (@AggregateId, @SequenceNumber, @TimeStamp,
                                @EventTypeName, @EventBody)
                        """;
            using var connection = dbConnection.Create();

            connection.Open();
            using var transaction = connection.BeginTransaction();

            connection.Execute(insertCommand,
                _newEvents.Select(DatabaseEvent.FromStoredEvent),
                transaction);

            transaction.Commit();

            foreach(var @event in _newEvents)
            {
                notificationService.PublishEvent(@event.AggregateId, @event.EventData);
            }
            _newEvents.Clear();
        }
    }
}
