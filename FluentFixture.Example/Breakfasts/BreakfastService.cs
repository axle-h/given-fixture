using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FluentFixture.Example.Breakfasts
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

            if (!(request.BreakfastItems?.Any() ?? false))
            {
                throw new ArgumentException("All breakfasts must have breakfast items", nameof(request.BreakfastItems));
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
            var fullEnglishTypes = Enum.GetValues(typeof(BreakfastItemType)).Cast<BreakfastItemType>().OrderBy(x => x);
            var isFullEnglish = items.Select(x => x.Type).OrderBy(x => x).SequenceEqual(fullEnglishTypes);

            if (isFullEnglish)
            {
                return "Full English Breakfast";
            }
            
            var toast = items.FirstOrDefault(i => i.Type == BreakfastItemType.Toast);
            if (toast != null)
            {
                return $"{GetItemNames(items.Except(new [] { toast }))} on Toast";
            }

            return GetItemNames(items);
        }
    }
}
