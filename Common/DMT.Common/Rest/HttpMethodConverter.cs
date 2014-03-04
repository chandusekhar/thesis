using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    internal class HttpMethodConverter
    {
        public HttpMethod FromString(string methodStr)
        {
            HttpMethod method;
            if (!Enum.TryParse<HttpMethod>(methodStr, true, out method))
            {
                throw new InvalidOperationException("Not supported HTTP method: " + methodStr);
            }

            return method;
        }
    }
}
