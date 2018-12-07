using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GivenFixture.Example.Breakfasts
{
    public class BreakfastService
    {
        private static readonly ICollection<BreakfastItemType> FullEnglishTypes = Enum.GetValues(typeof(BreakfastItemType))
                                                                                      .Cast<BreakfastItemType>()
                                                                                      .OrderBy(x => x)
                                                                                      .ToList();

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

            if (!(request.BreakfastItems?.Any() ?? false))
            {
                throw new ArgumentException("All breakfasts must have breakfast items", nameof(request));
            }

            var itemTasks = request.BreakfastItems.Distinct().Select(_breakfastItemRepository.GetBreakfastItemAsync);
            var items = await Task.WhenAll(itemTasks);

            return new Breakfast
                   {
                       Price = items.Sum(i => i.Price),
                       Name = GetBreakfastName(items)
                   };
        }

        private static string GetItemNames(IEnumerable<BreakfastItem> items) =>
            Regex.Replace(string.Join(", ", items.Select(i => i.Name)), ",(?=[^,]*$)", " and");

        private static string GetBreakfastName(ICollection<BreakfastItem> items)
        {
            var itemTypes = items.Select(x => x.Type).ToList();

            if (FullEnglishTypes.All(itemTypes.Contains))
            {
                return "Full English Breakfast";
            }
            
            if (itemTypes.Contains(BreakfastItemType.Toast))
            {
                var notToast = items.Where(x => x.Type != BreakfastItemType.Toast).ToList();
                var toast = items.Except(notToast).First();
                return $"{GetItemNames(notToast)} on {toast.Name}";
            }

            return GetItemNames(items);
        }
    }
}
