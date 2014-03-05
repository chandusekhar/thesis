using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class PatternCollection
    {
        private PatternSegment root;

        public PatternCollection()
        {
            this.root = PatternSegment.Create(string.Empty);
        }

        public PatternSegment Add(string urlPattern)
        {
            var segments = SplitUrl(urlPattern);
            // root does not need to be created
            return this.root.Add(segments.Skip(1));
        }

        public PatternSegment GetRoute(string route, NameValueCollection urlParams)
        {
            var segments = SplitUrl(route);
            return this.root.Get(segments, urlParams);
        }

        private IEnumerable<RouteSegment> SplitUrl(string url)
        {
            return new UrlSanitizer().Sanitize(url).Split('/').Select(s => new RouteSegment(s)).ToList();
        }
    }
}
