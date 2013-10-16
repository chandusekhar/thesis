using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test
{
    public class EdgeTest
    {
        [Fact]
        public void AfterSerializationEdgeHasStartNode()
        {
            Edge e = new Edge(new Node(), new Node());

            var doc = SerializerHelper.SerializeObject(e);

            Assert.NotEmpty(doc.Descendants(Edge.StartNodeTag));
        }

        [Fact]
        public void AfterSerializationEdgeHasEndNode()
        {
            Edge e = new Edge(new Node(), new Node());

            var doc = SerializerHelper.SerializeObject(e);

            Assert.NotEmpty(doc.Descendants(Edge.EndNodeTag));
        }

    }
}
