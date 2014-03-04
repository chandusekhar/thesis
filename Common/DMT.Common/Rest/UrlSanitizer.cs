using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    class UrlSanitizer
    {
        public string Sanitize(string url)
        {
            int index = url.IndexOf('?');
            if (index >-1) 
            {
                url = url.Substring(0, index);
            }
            return url.TrimEnd('/');
        }
    }
}
