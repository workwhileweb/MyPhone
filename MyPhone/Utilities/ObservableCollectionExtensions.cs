using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodTimeStudio.MyPhone.Utilities
{
    public static class ObservableCollectionExtensions
    {
        public static int Remove<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            var itemsToRemove = collection.Where(predicate).ToList();
            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }
            return itemsToRemove.Count;
        }
    }
}
