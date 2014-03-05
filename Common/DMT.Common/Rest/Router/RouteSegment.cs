using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class RouteSegment
    {
        private readonly static Regex ParamPattern = new Regex(@"\{(\w+)\}", RegexOptions.Compiled);

        private List<RouteSegment> subSegments;
        private readonly string value;

        protected virtual bool IsParam { get { return false; } }

        public HandlerBundle Handlers { get; private set; }

        public RouteSegment(string value)
        {
            this.value = value;
            this.subSegments = new List<RouteSegment>();
            this.Handlers = new HandlerBundle();
        }

        public static RouteSegment Create(string value)
        {
            var match = ParamPattern.Match(value);
            if (match.Success)
            {
                return new ParamRouteSegment(match.Groups[1].Value);
            }

            return new RouteSegment(value);
        }

        public RouteSegment Add(IEnumerable<string> segments)
        {
            var next = segments.FirstOrDefault();

            // no more segments left, self is the leaf
            if (next == null)
            {
                return this;
            }

            return FindOrCreate(next).Add(segments.Skip(1));
        }

        public RouteSegment Get(IEnumerable<string> segments, NameValueCollection urlParams)
        {
            bool hasNext = segments.Skip(1).Any();
            string current = segments.First();
            bool match = IsMatch(current);

            // no match, so return null
            if (!match)
            {
                return null;
            }

            if (this.IsParam)
            {
                urlParams.Add(this.value, current);
            }

            // no more segments to check, and match must be true here
            if (!hasNext)
            {
                // found it, yay!
                return this;
            }

            // more segments to check and the current matches
            return GetNext(segments, urlParams);
        }

        protected virtual bool IsMatch(string value)
        {
            return this.value == value;
        }

        private RouteSegment FindOrCreate(string value)
        {
            var subSegment = subSegments.Find(s => s.value == value);
            if (subSegment == null)
            {
                subSegment = RouteSegment.Create(value);
                this.subSegments.Add(subSegment);
            }

            return subSegment;
        }

        private RouteSegment GetNext(IEnumerable<string> segments, NameValueCollection urlParams)
        {
            string next = segments.Skip(1).First();

            // look for real route segment match
            var nextExactSegment = this.subSegments.Find(s => s.value == next && !s.IsParam);
            if (nextExactSegment != null)
            {
                // we got one, so match search *only* its subtree
                return nextExactSegment.Get(segments.Skip(1), urlParams);
            }

            RouteSegment matchedSegment;
            // no exact match, check for params
            foreach (var possibleMatch in this.subSegments.FindAll(s => s.IsParam))
            {
                // try to match the next registered param
                matchedSegment = possibleMatch.Get(segments.Skip(1), urlParams);
                if (matchedSegment != null)
                {
                    return matchedSegment;
                }
            }

            return null;
        }
    }
}
