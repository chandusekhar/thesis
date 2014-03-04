using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public struct HttpMethod
    {
        private static readonly string[] AllowedMethods = {
            "GET", "POST", "PUT", "DELETE"
        };

        public static readonly HttpMethod Get = "GET";
        public static readonly HttpMethod Post = "POST";
        public static readonly HttpMethod Put = "PUT";
        public static readonly HttpMethod Delete = "DELETE";

        private readonly string Method;

        public HttpMethod(string method)
        {
            var m = method.ToUpper();
            if (AllowedMethods.Any(s => s == m))
            {
                this.Method = m;
            }
            else
            {
                throw new ArgumentException("Method name not allowed: " + method, "method");
            }
        }

        public static implicit operator HttpMethod(string method)
        {
            return new HttpMethod(method);
        }

        public static implicit operator string(HttpMethod method)
        {
            return method.Method;
        }

        
    }
}
