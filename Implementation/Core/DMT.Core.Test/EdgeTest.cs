using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Exceptions;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Serialization;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test
{
    public class EdgeTest
    {
        [Fact]
        public void AfterSerializationEdgeHasStartNode()
        {
            Edge e = EntityHelper.CreateEdgeConnectingNodes();

            var doc = SerializerHelper.SerializeObject(e);

            Assert.NotEmpty(doc.Descendants(Edge.EndATag));
        }

        [Fact]
        public void AfterSerializationEdgeHasEndNode()
        {
            Edge e = EntityHelper.CreateEdgeConnectingNodes();

            var doc = SerializerHelper.SerializeObject(e);

            Assert.NotEmpty(doc.Descendants(Edge.EndBTag));
        }

        [Fact]
        public void AfterDeserializationEdgeHasTheSameStartNode()
        {
            Edge e, e2;
            SerializeAndDeserializeEdgeWithContext(out e, out e2);

            Assert.Equal(e.EndA.Id, e2.EndA.Id);
        }

        [Fact]
        public void AfterDeserializationEdgeHasTheSameEndNode()
        {
            Edge e, e2;
            SerializeAndDeserializeEdgeWithContext(out e, out e2);

            Assert.Equal(e.EndB.Id, e2.EndB.Id);
        }

        [Fact]
        public void ToStringContainsClassNameAndId()
        {
            Edge e = new Edge(null, null, Interfaces.EdgeDirection.Both, new CoreEntityFactory());
            var toString = string.Format("DMT.Core.Entities.Edge [{0}]", e.Id);
            Assert.Equal(toString, e.ToString());
        }

        [Fact]
        public void RemovingEdgeRemovesItFromNodesEdgeCollection()
        {
            Edge e = EntityHelper.CreateEdgeConnectingNodes();
            var n1 = e.EndA;
            var n2 = e.EndB;

            e.Remove();

            Assert.DoesNotContain(e, n1.Edges);
            Assert.DoesNotContain(e, n2.Edges);
        }

        [Fact]
        public void RemovingEdgeSetsNullToStartAndEnd()
        {
            Edge e = EntityHelper.CreateEdgeConnectingNodes();
            e.Remove();

            Assert.Null(e.EndA);
            Assert.Null(e.EndB);
        }

        [Fact]
        public void RemovingNotConnectedEdgeReturnsFalse()
        {
            Edge e = new Edge(null, null, Interfaces.EdgeDirection.Both, new CoreEntityFactory());
            Assert.Equal(false, e.Remove());
        }

        #region private helper methods

        private static void SerializeAndDeserializeEdgeWithContext(out Edge e, out Edge e2)
        {
            DeserializationContext ctx = new DeserializationContext(new CoreEntityFactory());

            e = EntityHelper.CreateEdgeConnectingNodes();
            ctx.AddNodes(e.EndA, e.EndB);

            e2 = SerializerHelper.SerializeAndDeserialize(e, ctx);
        }

        #endregion
    }
}
