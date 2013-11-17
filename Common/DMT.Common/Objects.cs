using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common
{
    public static class Objects
    {
        public static void RequireNonNull(object obj, string message = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message ?? "Object must not be null.", (Exception)null);
            }
        }
    }
}
