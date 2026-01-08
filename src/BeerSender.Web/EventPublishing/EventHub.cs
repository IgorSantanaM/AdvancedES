using Microsoft.AspNetCore.SignalR;

namespace BeerSender.Web.EventPublishing
{
    public class EventHub : Hub
    {
        public async Task PublishEvent(Guid aggregateId, object @event)
        {
            await Clients.Groups(aggregateId.ToString())
                .SendAsync("PublishEvent", aggregateId, @event);
        }

        public async Task SubscribeToAggregate(Guid aggregateId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, aggregateId.ToString());
        }
    }
}
