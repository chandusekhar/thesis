using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Extensions
{
    public static class StringExtensions
    {
        public static Guid ParseGuid(this string str)
        {
            if (str == null)
            {
                return Guid.Empty;
            }

            return Guid.Parse(str);
        }
    }
}
