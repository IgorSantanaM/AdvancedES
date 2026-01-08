using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Domain
{
    public interface INotificationService
    {
        public void PublishEvent(Guid aggregateId, object @event);
    }
}
