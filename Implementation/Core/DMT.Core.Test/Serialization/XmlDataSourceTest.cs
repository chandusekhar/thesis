using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Core.Exceptions;
using DMT.Core.Serialization;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test.Serialization
{
    public class XmlDataSourceTest
    {
        [Fact]
        public void LoadModelFromXml()
        {
            var ds = new XmlDataSource(new CoreEntityFactory());
            var model = ds.LoadModelAsync(File.OpenRead("model.xml")).Result;
            // model.xml has 3 nodes
            Assert.Equal(3, model.Nodes.Count());
        }

        [Fact]
        public void EmptyModelThrownFormatException()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("<Model></Model>"));
            var ds = new XmlDataSource(new CoreEntityFactory());

            AssertEx.ThrowsAsync(typeof(ModelXmlFormatException), () => ds.LoadModelAsync(stream));
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
            Assert.Equal(XmlDataSource.RootTag, doc.Root.Name);
        }

        [Fact]
        public void SavedXmlHasTwoNodes()
        {
            var doc = SaveModel();
            Assert.Equal(2, doc.Descendants(XmlDataSource.NodeTag).Count());
        }

        [Fact]
        public void SavedXmlHasOneEdge()
        {
            var doc = SaveModel();
            Assert.Equal(1, doc.Descendants(XmlDataSource.EdgeTag).Count());
        }


        private static XDocument SaveModel()
        {
            Node n1 = EntityHelper.CreateNode(), n2 = EntityHelper.CreateNode();
            n1.ConnectTo(n2, Interfaces.EdgeDirection.Both);
            Model m = new Model(new Node[] { n1, n2 });
            MemoryStream target = new MemoryStream();
            var ds = new XmlDataSource(new CoreEntityFactory());

            ds.SaveModelAsync(target, m).Wait();

            target.Seek(0, SeekOrigin.Begin);
            var doc = XDocument.Load(target);
            return doc;
        }

    }
}
