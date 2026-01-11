using BeerSender.Domain.Boxes;
using JasperFx.Events;
using Marten.Events;
using Marten.Events.Projections;

namespace BeerSender.Domain.Projections
{
    public class OpenBox
    {
        public Guid BoxId { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBottles { get; set; }
    }

    public class OpenBoxProjection : EventProjection
    {
        // Mapping relevant events for the projection
        public OpenBoxProjection()
        {
            // Verifies if the BoxCreated event has been stored, if yes do the projection.
            Project<IEvent<BoxCreated>>((evt, operations) =>
            {
                operations.Store(new OpenBox
                {
                    BoxId = evt.StreamId,
                    Capacity = evt.Data.Capacity.NumberOfSpots,
                });
            });

            // Verifies if the BoxClosed event has been stored, if yes do the projection.
            Project<IEvent<BoxClosed>>((evt, operations) =>
            {
                operations.Delete<OpenBox>(evt.StreamId);
            });

            // Verifies if the BoxClosed event has been stored, if yes do the projection.
            ProjectAsync<IEvent<BeerBottleAdded>>(async (evt, operations, token) =>
            {
                if(token.IsCancellationRequested) return;

                var openBox = await operations.LoadAsync<OpenBox>(evt.StreamId);

                if(openBox is null)
                    return;

                openBox.NumberOfBottles++;
                operations.Store(openBox);
            });
        }
    }
}
