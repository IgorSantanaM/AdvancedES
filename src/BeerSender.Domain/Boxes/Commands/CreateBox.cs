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
        var stream = await session.Events.FetchForWriting<Box>(command.BoxId);

        var capacity = BoxCapacity.Create(command.DesiredNumberOfSpots);

        stream.AppendOne(new BoxCreated(capacity, command.FriendlyName, command.ContainerType));
    }
}