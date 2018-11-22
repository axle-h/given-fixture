using System.Threading.Tasks;

namespace GivenFixture.Example.Breakfasts
{
    public interface IBreakfastItemRepository
    {
        Task<BreakfastItem> GetBreakfastItemAsync(BreakfastItemType itemType);
    }
}