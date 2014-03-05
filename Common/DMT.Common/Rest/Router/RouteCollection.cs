using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class RouteCollection
    {
        private RouteSegment root;

        public RouteCollection()
        {
            this.root = RouteSegment.Create(string.Empty);
        }

        public RouteSegment Add(string urlPattern)
        {
            var segments = SplitUrl(urlPattern);
            // root does not need to be created
            return this.root.Add(segments.Skip(1));
        }

        public RouteSegment GetRoute(string route, NameValueCollection urlParams)
        {
            var segments = SplitUrl(route);
            return this.root.Get(segments, urlParams);
        }

        private IEnumerable<string> SplitUrl(string url)
        {
            return new UrlSanitizer().Sanitize(url).Split('/');
        }
    }
}
