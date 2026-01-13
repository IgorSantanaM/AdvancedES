using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BeerSender.Domain.Tests.Boxes;

[Collection("Marten collection")]
public class AddBeerHandlerTest(MartenFixture fixture) 
    : BoxTests<AddBeerBottle>(fixture)
{
    protected override ICommandHandler<AddBeerBottle> Handler => new AddBeerBottleHandler();

    [Fact]
    public async Task AddBeerBottle_ShouldAddBottle_WhenBoxIsEmpty()
    {
        const int NUMBER_OF_BOTTLES = 6;

        // Arrange
        await Given<Box>(
            Box_created_with_capacity(NUMBER_OF_BOTTLES)
            );

        // Act
        await When(
            Add_beer_Bottle(carte_blanche)
        );

        // Assert
        await Then(
           Beer_bottle_added(carte_blanche)
        );
    }
    private AddBeerBottle Add_beer_Bottle(BeerBottle bottle)
    {
        return new AddBeerBottle(Box_Id, bottle);
    }
}
