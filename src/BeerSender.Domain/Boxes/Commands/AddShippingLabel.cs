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
);

public class AddShippingLabelHandler(IDocumentStore store) : ICommandHandler<AddShippingLabel>
{
    public async Task Handle(AddShippingLabel command)
    {
        await using var session = store.IdentitySession();

        var box = await session.Events.AggregateStreamAsync<Box>(command.BoxId);

        if (command.Label.IsValid())
            session.Events.Append(command.BoxId, new ShippingLabelAdded(command.Label));

        else
            session.Events.Append(command.BoxId, new FailedToAddShippingLabel(
                FailedToAddShippingLabel.FailReason.TrackingCodeInvalid));

        await session.SaveChangesAsync();
    }
}