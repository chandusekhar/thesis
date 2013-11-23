using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddAll<T>(this ICollection<T> self, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                self.Add(item);
            }
        }
    }
}
