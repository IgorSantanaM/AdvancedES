namespace BeerSender.Domain.Boxes;

public class Box
{
    public Guid Id { get; set; }
    public List<BeerBottle> BeerBottles { get; set; } = [];
    public BoxCapacity? BoxType { get; set; }
    public ShippingLabel? ShippingLabel { get; set; }
    public bool IsClosed { get; set; }
    public bool IsSent { get; set; }

    public void Apply(BoxCreated @event)
    {
        BoxType = @event.BoxType;
    }

    public void Apply(BeerBottleAdded @event)
    {
        BeerBottles.Add(@event.Bottle);
    }

    public void Apply(ShippingLabelAdded @event)
    {
        ShippingLabel = @event.Label;
    }

    public void Apply(BoxClosed @event)
    {
        IsClosed = true;
    }

    public void Apply(BoxSent @event)
    {
        IsSent = true;
    }

    public bool IsFull => BeerBottles.Count >= BoxType?.NumberOfSpots;
}