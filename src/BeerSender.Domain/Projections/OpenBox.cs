using BeerSender.Domain.Boxes;
using JasperFx.Events.Aggregation;
using Marten.Events.Aggregation;

namespace BeerSender.Domain.Projections
{
    public class OpenBox
    {
        public Guid BoxId { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBottles { get; set; }
    }

    public class OpenBoxProjection : SingleStreamProjection<OpenBox, Guid>
    {
        public OpenBoxProjection()
        {
            DeleteEvent<BoxClosed>();
        }

        public static OpenBox Create(BoxCreated started)
        {
            return new OpenBox
            {
                Capacity = started.BoxType.NumberOfSpots
            };
        }

        public static void Apply(BeerBottleAdded _, OpenBox box)
        {
            box.NumberOfBottles++;
        }
    }
}
