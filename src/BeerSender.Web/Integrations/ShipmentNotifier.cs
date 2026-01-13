using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using Marten.Subscriptions;

namespace BeerSender.Web.Integrations
{
    public class ShipmentNotifier(ILogger<ShipmentNotifier> logger) : ISubscription
    {
        public Task<IChangeListener> ProcessEventsAsync(EventRange page, ISubscriptionController controller, IDocumentOperations operations, CancellationToken cancellationToken)
        {
            foreach (var @event in page.Events)
            {

                // Called everytime boxsent event is stored
                throw new Exception("Failed call");
                logger.LogDebug("Called Carrier's API to notify for pickup");
            }

            return Task.FromResult(NullChangeListener.Instance);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}
