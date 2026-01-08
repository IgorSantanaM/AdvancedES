using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;
public class AddBeerHandlerTest : BoxTests<AddBeerBottle>
{
    protected override CommandHandler<AddBeerBottle> Handler => new AddBeerBottleHandler(eventStore);

    [Fact]
    public void AddBeerBottle_ShouldAddBottle_WhenBoxIsEmpty()
    {
        const int NUMBER_OF_BOTTLES = 6;

        // Arrange
        Given(
            Box_created_with_capacity(NUMBER_OF_BOTTLES)
            );

        // Act
        When(
            Add_beer_Bottle(carte_blanche)
        );

        // Assert
        Then(
           Beer_bottle_added(carte_blanche)
        );
    }
    private AddBeerBottle Add_beer_Bottle(BeerBottle bottle)
    {
        return new AddBeerBottle(Box_Id, bottle);
    }
}
