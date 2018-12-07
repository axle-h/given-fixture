using System.Collections.Generic;

namespace GivenFixture.Example.Breakfasts
{
    public class GetBreakfastRequest
    {
        public ICollection<BreakfastItemType> BreakfastItems { get; set; }
    }
}