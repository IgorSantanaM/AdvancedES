using Marten;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BeerSender.Domain.Boxes.Commands;

public record AddShippingLabel(
    Guid BoxId,
    ShippingLabel Label
) : ICommand;

public class AddShippingLabelHandler() : ICommandHandler<AddShippingLabel>
{
    public async Task Handle(IDocumentSession session, AddShippingLabel command)
    {
        var box = await session.Events.AggregateStreamAsync<Box>(command.BoxId);

        if (command.Label.IsValid())
            session.Events.Append(command.BoxId, new ShippingLabelAdded(command.Label));

        else
            session.Events.Append(command.BoxId, new FailedToAddShippingLabel(
                FailedToAddShippingLabel.FailReason.TrackingCodeInvalid));
    }
}