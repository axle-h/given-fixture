using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
        public Task When_getting_bacon_egg_and_sausage() =>
            Given.Fixture
                 .HavingBreakfastItem(BreakfastItemType.Bacon, out var bacon)
                 .HavingBreakfastItem(BreakfastItemType.Egg, out var egg)
                 .HavingBreakfastItem(BreakfastItemType.Sausage, out var sausage)
                 .WhenGettingBreakfast(BreakfastItemType.Bacon, BreakfastItemType.Egg, BreakfastItemType.Sausage)
                 .ShouldReturnBreakfastWithCorrectNameAndPrice("Bacon, Egg and Sausage", bacon, egg, sausage)
                 .RunAsync();
    }

    internal static class BreakfastTestExtensions
    {
        public static ITestFixture HavingBreakfastItem(this ITestFixture fixture, BreakfastItemType type, out BreakfastItem item) =>
            fixture.HavingModel(out item, c => c.With(x => x.Type, type)
                                                .With(x => x.Name, type.ToString()))
                   .HavingMocked<IBreakfastItemRepository, BreakfastItem>(x => x.GetBreakfastItemAsync(type), item);


        public static ITestFixture WhenGettingBreakfast(this ITestFixture fixture, params BreakfastItemType[] types) =>
            fixture.When<BreakfastService, Breakfast>(x => x.GetBreakfastAsync(new GetBreakfastRequest { BreakfastItems = types }));

        public static ITestFixture ShouldReturnBreakfastWithCorrectNameAndPrice(this ITestFixture fixture,
                                                                                string expectedName,
                                                                                params BreakfastItem[] expectedItems) =>
            fixture.ShouldReturnEquivalent(new Breakfast { Name = expectedName, Price = expectedItems.Sum(i => i.Price) });
    }
}
