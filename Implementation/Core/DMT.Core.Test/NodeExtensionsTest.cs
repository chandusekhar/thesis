using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DMT.Core.Extensions;
using DMT.Core.Entities;

namespace DMT.Core.Test
{
    public class NodeExtensionsTest
    {
        [Fact]
        public void GetAdjacentNodes()
        {
            Node n1 = new Node();
            Edge e1 = new Edge(n1, new Node());
            Edge e2 = new Edge(n1, new Node());

            var neighbours = n1.AdjacentNodes();

            Assert.Contains(e1.Target, neighbours);
            Assert.Contains(e1.Target, neighbours);
        }

    }
}
