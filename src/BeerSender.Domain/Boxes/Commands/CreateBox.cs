using Marten;
using Marten.Internal.Sessions;

namespace BeerSender.Domain.Boxes.Commands;

public record CreateBox(
    Guid BoxId,
    int DesiredNumberOfSpots,
    string? FriendlyName,
    ContainerType ContainerType
) : ICommand;

public class CreateBoxHandler()
    : ICommandHandler<CreateBox>
{
    public async Task Handle(IDocumentSession session, CreateBox command)
    {
        var capacity = BoxCapacity.Create(command.DesiredNumberOfSpots);

        session.Events.StartStream<Box>(command.BoxId, new BoxCreated(capacity, command.FriendlyName, command.ContainerType));
    }
}