namespace BeerSender.Domain.Boxes.Commands;

public record SendBox(
    Guid BoxId
);

public class SendBoxHandler(IEventStore eventStore)
    : CommandHandler<SendBox>(eventStore)
{
    public override void Handle(SendBox command)
    {
        var boxStream = GetStream<Box>(command.BoxId);
        var box = boxStream.GetEntity();

        var success = true;

        if (!box.IsClosed)
        {
            boxStream.Append(new FailedToSendBox(FailedToSendBox.FailReason.BoxWasNotClosed));
            success = false;
        }

        if (box.ShippingLabel is null)
        {
            boxStream.Append(new FailedToSendBox(FailedToSendBox.FailReason.BoxHadNoLabel));
            success = false;
        }

        if (success)
        {
            boxStream.Append(new BoxSent());
        }
    }
}