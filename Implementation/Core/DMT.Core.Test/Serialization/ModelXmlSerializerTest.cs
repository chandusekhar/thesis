using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Serialization;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test.Serialization
{
    public class ModelXmlSerializerTest
    {
        ModelXmlSerializer serializer = new ModelXmlSerializer(new CoreEntityFactory());

        [Fact]
        public void LoadModelFromXml()
        {
            using (var reader = XmlReader.Create("model.xml"))
            {
                var model = serializer.Deserialize(reader);
                // model.xml has 3 nodes
                Assert.Equal(3, model.Nodes.Count());
            }
        }

        [Fact]
        public void EmptyModelThrownFormatException()
        {
            using (var r = XmlReader.Create(new StringReader("<Model></Model>")))
            {
                Assert.Throws(typeof(ModelXmlFormatException), () => serializer.Deserialize(r));
            }
        }


        [Fact]
        public void SavingModelProducesValidXml()
        {
            var doc = SaveModel();

            // if this is reached that means that the xml was valid, bacause it could be parsed.
            Assert.NotNull(doc);
        }

        [Fact]
        public void SavedXMLHasRootWithNameModel()
        {
            var doc = SaveModel();
            Assert.Equal(ModelXmlSerializer.RootTag, doc.Root.Name);
        }

        [Fact]
        public void SavedXmlHasTwoNodes()
        {
            var doc = SaveModel();
            Assert.Equal(2, doc.Descendants(ModelXmlSerializer.NodeTag).Count());
        }

        [Fact]
        public void SavedXmlHasOneEdge()
        {
            var doc = SaveModel();
            Assert.Equal(1, doc.Descendants(ModelXmlSerializer.EdgeTag).Count());
        }


        private XDocument SaveModel()
        {
            Node n1 = EntityHelper.CreateNode(), n2 = EntityHelper.CreateNode();
            n1.ConnectTo(n2, Interfaces.EdgeDirection.Both);
            Model m = new Model(new Node[] { n1, n2 });
            MemoryStream target = new MemoryStream();

            using (var w = XmlWriter.Create(target))
            {
                serializer.Serialize(w, m);
            }

            target.Seek(0, SeekOrigin.Begin);
            var doc = XDocument.Load(target);
            return doc;
        }

    }
}
