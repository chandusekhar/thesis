using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Matcher.Module.Service;
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
            var ns = new NodeSerializer(ms, f);
            ns.Serialize(f.CreateNode());

            ms.Position = 0;
            var doc = XDocument.Load(ms);
            Assert.NotNull(doc.Element("Node"));
        }

        [Fact]
        public void TestDeserializeNode()
        {
            MemoryStream ms = new MemoryStream();
            var f = new CoreEntityFactory();
            var ns = new NodeSerializer(ms, f);
            var nodeOrig = f.CreateNode();
            ns.Serialize(nodeOrig);

            ms.Position = 0;
            var node = ns.Deserialize();
            Assert.Equal(nodeOrig.Id, node.Id);
        }
    }
}
