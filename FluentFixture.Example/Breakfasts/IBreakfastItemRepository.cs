using System.Threading.Tasks;

namespace FluentFixture.Example.Breakfasts
{
    public interface IBreakfastItemRepository
    {
        Task<BreakfastItem> GetBreakfastItemAsync(BreakfastItemType itemType);
    }
}