using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentFixture.Example.Breakfasts;
using Moq;
using Xunit;

namespace FluentFixture.Example
{
    public class BreakfastServiceTests
    {
        [Fact]
        public void When_attempting_to_get_breakfast_with_null_request()
        {
            using (var fixture = new BreakfastServiceFixture())
            {
                fixture.HavingNullGetBreakfastRequest()
                       .WhenGettingBreakfast()
                       .ShouldThrow<ArgumentNullException>();
            }
        }

        [Fact]
        public void When_attempting_to_get_breakfast_with_no_items()
        {
            using (var fixture = new BreakfastServiceFixture())
            {
                fixture.HavingGetBreakfastRequest()
                       .WhenGettingBreakfast()
                       .ShouldThrow<ArgumentException>();
            }
        }

        [Fact]
        public void When_getting_full_english_breakfast()
        {
            using (var fixture = new BreakfastServiceFixture())
            {
                fixture.HavingGetBreakfastRequest(BreakfastItemType.Bacon,
                                                  BreakfastItemType.Egg,
                                                  BreakfastItemType.Sausage,
                                                  BreakfastItemType.Toast)
                       .WhenGettingBreakfast()
                       .ShouldReturnBreakfastWithCorrectNameAndPrice("Full English Breakfast");
            }
        }

        [Fact]
        public void When_getting_bacon_and_egg_pn_toast()
        {
            using (var fixture = new BreakfastServiceFixture())
            {
                fixture.HavingGetBreakfastRequest(BreakfastItemType.Bacon,
                                                  BreakfastItemType.Egg,
                                                  BreakfastItemType.Toast)
                       .WhenGettingBreakfast()
                       .ShouldReturnBreakfastWithCorrectNameAndPrice("Bacon and Egg on Toast");
            }
        }

        [Fact]
        public void When_getting_bacon_egg_and_sausage()
        {
            using (var fixture = new BreakfastServiceFixture())
            {
                fixture.HavingGetBreakfastRequest(BreakfastItemType.Bacon,
                                                  BreakfastItemType.Egg,
                                                  BreakfastItemType.Sausage)
                       .WhenGettingBreakfast()
                       .ShouldReturnBreakfastWithCorrectNameAndPrice("Bacon, Egg and Sausage");
            }
        }

        private class BreakfastServiceFixture : TestFixture<BreakfastService>
        {
            private readonly IList<BreakfastItem> _items = new List<BreakfastItem>();
            private GetBreakfastRequest _getBreakfastRequest;

            public BreakfastServiceFixture HavingNullGetBreakfastRequest()
            {
                _getBreakfastRequest = null;
                return this;
            }

            public BreakfastServiceFixture HavingGetBreakfastRequest(params BreakfastItemType[] itemTypes)
            {
                _getBreakfastRequest = AutoFixture.Build<GetBreakfastRequest>()
                                                  .With(x => x.BreakfastItems, itemTypes)
                                                  .Create();

                foreach (var type in itemTypes)
                {
                    var item = AutoFixture.Build<BreakfastItem>()
                                          .With(x => x.Type, type)
                                          .With(x => x.Name, type.ToString())
                                          .Create();

                    Mock<IBreakfastItemRepository>()
                       .Setup(x => x.GetBreakfastItemAsync(type))
                       .ReturnsAsync(item)
                       .Verifiable();

                    _items.Add(item);
                }

                return this;
            }

            public BreakfastServiceFixture WhenGettingBreakfast()
            {
                ActAsync(x => x.GetBreakfastAsync(_getBreakfastRequest));
                return this;
            }

            public void ShouldReturnBreakfastWithCorrectNameAndPrice(string name) =>
                Assert<Breakfast>(x => x.Name.Should().Be(name),
                                  x => x.Price.Should().Be(_items.Sum(i => i.Price)));
        }
    }
}
