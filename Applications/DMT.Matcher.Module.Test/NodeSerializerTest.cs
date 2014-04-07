using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Matcher.Module.Service;
using DMT.VIR.Data;
using Xunit;

namespace DMT.Matcher.Module.Test
{
    public class NodeSerializerTest
    {
        [Fact]
        public void TestNodeSerialization()
        {
            MemoryStream ms = new MemoryStream();
            var f = new CoreEntityFactory();
            var ns = new NodeSerializer(ms, f, new DummyContextFactory());
            ns.Serialize(f.CreateNode());

            ms.Position = 0;
            var doc = XDocument.Load(ms);
            Assert.Single(doc.Descendants("Node"));
        }

        [Fact]
        public void TestDeserializeNode()
        {
            MemoryStream ms = new MemoryStream();
            var f = new CoreEntityFactory();
            var ns = new NodeSerializer(ms, f, new DummyContextFactory());
            var nodeOrig = f.CreateNode();
            ns.Serialize(nodeOrig);

            ms.Position = 0;
            var node = ns.Deserialize();
            Assert.Equal(nodeOrig.Id, node.Id);
        }

        [Fact]
        public void TestEdgesSerialization()
        {
            MemoryStream ms = new MemoryStream();
            var f = new VirEntityFactory();
            var ns = new NodeSerializer(ms, f, new DummyContextFactory());

            var node = f.CreateNode();
            node.ConnectTo(f.CreateNode(), Core.Interfaces.EdgeDirection.Both);

            ns.Serialize(node);

            ms.Position = 0;
            Assert.Single(XDocument.Load(ms).Descendants("Edge"));
        }

        [Fact]
        public void TestEdgeDeserialization()
        {
            MemoryStream ms = new MemoryStream();
            var f = new VirEntityFactory();
            var ns = new NodeSerializer(ms, f, new DummyContextFactory());

            var node = new Person(f);
            node.ConnectTo(f.CreateNode(), Core.Interfaces.EdgeDirection.Both);

            ns.Serialize(node);

            ms.Position = 0;
            var nOut = ns.Deserialize();

            Assert.Single(nOut.Edges);
            Assert.IsAssignableFrom<IRemoteNode>(nOut.Edges.Single().GetOtherNode(nOut));
        }

        private class DummyContextFactory : IContextFactory
        {

            public IContext CreateContext()
            {
                return null;
            }
        }
    }
}
