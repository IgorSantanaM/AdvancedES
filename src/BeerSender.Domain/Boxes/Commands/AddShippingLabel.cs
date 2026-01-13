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
        var stream = await session.Events.FetchForWriting<Box>(command.BoxId);
        var box = stream.Aggregate;

        if (command.Label.IsValid())
            stream.AppendOne(new ShippingLabelAdded(command.Label));

        else
            stream.AppendOne(new FailedToAddShippingLabel(FailedToAddShippingLabel.FailReason.TrackingCodeInvalid));
    }
}