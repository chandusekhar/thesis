using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static T SafeGet<T>(this T[] array, int index, T @default = default(T))
        {
            if (array.Length > index)
            {
                return array[index];
            }

            return @default;
        }

        public static bool TryGet<T>(this T[] array, int index, out T value)
        {
            if (array.Length > index)
            {
                value = array[index];
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
