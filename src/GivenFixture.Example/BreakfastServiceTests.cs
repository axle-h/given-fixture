using System.Linq;
using System.Threading.Tasks;
using GivenFixture.Extensions;
using GivenFixture.Example.Breakfasts;
using Xunit;

namespace GivenFixture.Example
{
    public class BreakfastServiceTests
    {
        [Fact]
        public Task When_attempting_to_get_breakfast_with_null_request() =>
            Given.Fixture
                 .When<BreakfastService, Breakfast>(x => x.GetBreakfastAsync(null))
                 .ShouldThrowArgumentNullException("request")
                 .RunAsync();

        [Fact]
        public Task When_attempting_to_get_breakfast_with_no_items() =>
            Given.Fixture
                 .WhenGettingBreakfast()
                 .ShouldThrowArgumentException("request")
                 .RunAsync();

        [Fact]
        public Task When_attempting_to_get_breakfast_with_missing_item() =>
            Given.Fixture
                 .HavingMockedAsync<IBreakfastItemRepository, BreakfastItem>(x => x.GetBreakfastItemAsync(BreakfastItemType.Bacon), null)
                 .WhenGettingBreakfast(BreakfastItemType.Bacon)
                 .ShouldReturnNull()
                 .RunAsync();

        [Fact]
        public Task When_getting_bacon_egg_and_sausage() =>
            Given.Fixture
                 .HavingBreakfastItem(BreakfastItemType.Bacon, out var bacon)
                 .HavingBreakfastItem(BreakfastItemType.Egg, out var egg)
                 .HavingBreakfastItem(BreakfastItemType.Sausage, out var sausage)
                 .WhenGettingBreakfast(BreakfastItemType.Bacon, BreakfastItemType.Egg, BreakfastItemType.Sausage)
                 .ShouldReturnBreakfastWithCorrectNameAndPrice("Bacon, Egg and Sausage", bacon, egg, sausage)
                 .RunAsync();

        [Fact]
        public Task When_getting_full_english_breakfast() =>
            Given.Fixture
                 .HavingBreakfastItem(BreakfastItemType.Bacon, out var bacon)
                 .HavingBreakfastItem(BreakfastItemType.Egg, out var egg)
                 .HavingBreakfastItem(BreakfastItemType.Sausage, out var sausage)
                 .HavingBreakfastItem(BreakfastItemType.Toast, out var toast)
                 .WhenGettingBreakfast(BreakfastItemType.Bacon, BreakfastItemType.Egg, BreakfastItemType.Sausage, BreakfastItemType.Toast)
                 .ShouldReturnBreakfastWithCorrectNameAndPrice("Full English Breakfast", bacon, egg, sausage, toast)
                 .RunAsync();

        [Fact]
        public Task When_getting_bacon_and_egg_on_toast() =>
            Given.Fixture
                 .HavingBreakfastItem(BreakfastItemType.Bacon, out var bacon)
                 .HavingBreakfastItem(BreakfastItemType.Egg, out var egg)
                 .HavingBreakfastItem(BreakfastItemType.Toast, out var toast)
                 .WhenGettingBreakfast(BreakfastItemType.Bacon, BreakfastItemType.Egg, BreakfastItemType.Toast)
                 .ShouldReturnBreakfastWithCorrectNameAndPrice("Bacon and Egg on Toast", bacon, egg, toast)
                 .RunAsync();

        [Fact]
        public Task When_getting_duplicate_bacon() =>
            Given.Fixture
                 .HavingBreakfastItem(BreakfastItemType.Bacon, out var bacon)
                 .WhenGettingBreakfast(BreakfastItemType.Bacon, BreakfastItemType.Bacon)
                 .ShouldReturnBreakfastWithCorrectNameAndPrice("Bacon", bacon)
                 .RunAsync();
    }

    internal static class BreakfastTestExtensions
    {
        /// <summary>
        /// Configures the mock breakfast item repository to return a relevant breakfast item
        /// when called with the specified breakfast item type.
        /// </summary>
        public static ITestFixture HavingBreakfastItem(this ITestFixture fixture, BreakfastItemType type, out BreakfastItem item) =>
            fixture.HavingMockedAsync<IBreakfastItemRepository, BreakfastItem>(x => x.GetBreakfastItemAsync(type),
                                                                               out item,
                                                                               (f, m) =>
                                                                               {
                                                                                   m.Type = type;
                                                                                   m.Name = type.ToString();
                                                                               });


        /// <summary>
        /// Configures the fixture to construct a <see cref="BreakfastService"/> subject
        /// and call <see cref="BreakfastService.GetBreakfastAsync"/> with the specified breakfast item types.
        /// </summary>
        public static ITestFixture WhenGettingBreakfast(this ITestFixture fixture, params BreakfastItemType[] types) =>
            fixture.When<BreakfastService, Breakfast>(x => x.GetBreakfastAsync(new GetBreakfastRequest { BreakfastItems = types }));

        /// <summary>
        /// Configures the fixture to assert that the subject returns a breakfast with the specified name
        /// and price as calculated from the specified breakfast items.
        /// </summary>
        public static ITestFixture ShouldReturnBreakfastWithCorrectNameAndPrice(this ITestFixture fixture,
                                                                                string expectedName,
                                                                                params BreakfastItem[] expectedItems) =>
            fixture.ShouldReturnEquivalent(new Breakfast { Name = expectedName, Price = expectedItems.Sum(i => i.Price) });
    }
}
