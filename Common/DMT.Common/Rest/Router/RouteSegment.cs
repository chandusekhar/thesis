using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class RouteSegment
    {
        private readonly string value;
        private bool isProcessed;

        public string Value
        {
            get { return this.value; }
        }

        public bool IsProcessed
        {
            get { return this.isProcessed; }
            set { this.isProcessed = value; }
        }

        public RouteSegment(string value)
        {
            this.value = value;
            this.isProcessed = false;
        }
    }
}
