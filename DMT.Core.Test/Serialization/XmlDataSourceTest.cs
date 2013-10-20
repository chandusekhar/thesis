using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Exceptions;
using DMT.Core.Serialization;
using Xunit;

namespace DMT.Core.Test.Serialization
{
    public class XmlDataSourceTest
    {
        [Fact]
        public void LoadModelFromXml()
        {
            var ds = new XmlDataSource();
            var roots = ds.LoadModelAsync().Result;
            Assert.Equal(1, roots.Count());
        }

        [Fact]
        public void EmptyModelThrownFormatException()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("<Model></Model>"));
            var ds = new XmlDataSource();
            try
            {
                var res = ds.UseStream(stream).LoadModelAsync().Result;
            }
            catch (AggregateException ex)
            {
                Assert.True(ex.InnerExceptions.Any(e=>e is ModelXmlFormatException));
            }
        }

    }
}
