[![Build status](https://ci.appveyor.com/api/projects/status/242cdlidfmyle6o2/branch/master?svg=true)](https://ci.appveyor.com/project/axle-h/fluent-fixture/branch/master)
[![NuGet](https://img.shields.io/nuget/v/FluentFixture.svg)](https://www.nuget.org/packages/FluentFixture/)

# fluent-fixture

Simple test fixture pattern.

Intended to be used to write tests like this:

```C#
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
```