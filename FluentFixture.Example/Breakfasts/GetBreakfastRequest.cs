using System.Collections.Generic;

namespace FluentFixture.Example.Breakfasts
{
    public class GetBreakfastRequest
    {
        public ICollection<BreakfastItemType> BreakfastItems { get; set; }
    }
}