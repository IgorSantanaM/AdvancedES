using BeerSender.Domain.Boxes;
using BeerSender.Domain.Projections;
using FluentAssertions;
using FluentAssertions.Extensions;
using Marten.Events;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Domain.Tests.Boxes.Projections
{
    public class OpenBoxTest(MartenFixture fixture) : MartenTest(fixture)
    {
        [Fact]
        public async Task OpenBoxProjection_ShouldProjectOpenBox_WhenBoxIsOpenWithBottles()
        {
            var boxId = Guid.NewGuid();

            object[] events =
            [
                Box_created_with_capacity(24),
                Beer_bottle_added(gouden_carolus)
            ];

            await using var session = Store.LightweightSession();

            session.Events.StartStream<Box>(boxId, events);

            await session.SaveChangesAsync();

            await Store.WaitForNonStaleProjectionDataAsync(5.Seconds());

            await using var querySesion = Store.QuerySession();

            var openBox = await querySesion.LoadAsync<OpenBox>(boxId);

            openBox.Should().NotBeNull();   
            openBox.BoxId.Should().Be(boxId);   
            openBox.Capacity.Should().Be(24);
            openBox.NumberOfBottles.Should().Be(1);
        }

        protected BoxCreated Box_created_with_capacity(int capacity)
        {
            return new(new BoxCapacity(capacity), string.Empty, ContainerType.Bottle);
        }

        protected BeerBottleAdded Beer_bottle_added(BeerBottle bottle)
        {
            return new BeerBottleAdded(bottle);
        }

        protected BeerBottle gouden_carolus = new("Gouden carlous", "Quadrupel Whisky", 12.7, BeerType.Quadruple);
    }
}
