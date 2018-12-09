using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GivenFixture.Example.Breakfasts
{
    public class BreakfastService
    {
        private readonly IBreakfastItemRepository _breakfastItemRepository;

        public BreakfastService(IBreakfastItemRepository breakfastItemRepository)
        {
            _breakfastItemRepository = breakfastItemRepository;
        }

        public async Task<Breakfast> GetBreakfastAsync(GetBreakfastRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Make sure we have some breakfast items.
            if (request.BreakfastItems == null || !request.BreakfastItems.Any())
            {
                throw new ArgumentException("All breakfasts must have breakfast items", nameof(request));
            }

            // Get the breakfast items.
            var itemTasks = request.BreakfastItems
                                   .Distinct()
                                   .Select(_breakfastItemRepository.GetBreakfastItemAsync);
            var items = await Task.WhenAll(itemTasks);

            // Ensure we have all items.
            if (items.Any(x => x == null))
            {
                return null;
            }

            // Make the breakfast.
            return new Breakfast
                   {
                       Price = items.Sum(i => i.Price),
                       Name = GetBreakfastName(items)
                   };
        }

        private static string GetBreakfastName(ICollection<BreakfastItem> items)
        {
            var itemTypes = items.Select(x => x.Type).ToList();

            // Check for full english breakfast.
            var isFullEnglish = Enum.GetValues(typeof(BreakfastItemType))
                                    .Cast<BreakfastItemType>()
                                    .All(itemTypes.Contains);
            if (isFullEnglish)
            {
                return "Full English Breakfast";
            }

            // Check for breakfast items on toast.
            if (itemTypes.Contains(BreakfastItemType.Toast))
            {
                var notToast = items.Where(x => x.Type != BreakfastItemType.Toast).ToList();
                var toast = items.Except(notToast).First();
                return $"{GetItemNames(notToast)} on {toast.Name}";
            }

            // Fall back to a list of all items.
            return GetItemNames(items);
        }

        private static string GetItemNames(IEnumerable<BreakfastItem> items) =>
            Regex.Replace(string.Join(", ", items.Select(i => i.Name)), ",(?=[^,]*$)", " and");
    }
}
