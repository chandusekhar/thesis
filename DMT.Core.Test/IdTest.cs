using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test
{
    public class IdTest
    {
        [Fact]
        public void IdEquality()
        {
            Guid g = Guid.NewGuid();
            DMTId id = DMTId.FromGuid(g);
            DMTId id2 = DMTId.FromGuid(g);

            Assert.True(id.Equals(id2));
        }

        [Fact]
        public void IdEqualityUsingBuiltInFunction()
        {
            Guid g = Guid.NewGuid();
            DMTId id = DMTId.FromGuid(g);
            DMTId id2 = DMTId.FromGuid(g);

            Assert.True(id.Equals((object)id2));
        }


        [Fact]
        public void IdNotEquiality()
        {
            Assert.False(DMTId.NewId().Equals(DMTId.NewId()));
        }

        [Fact]
        public void SerializeId()
        {
            var id = DMTId.NewId();
            XDocument doc = SerializerHelper.SerializeObject(id);
            Assert.Equal("root", doc.Root.Name);
            Assert.Equal(id, DMTId.FromGuid(Guid.Parse(doc.Root.Value)));
        }

        [Fact]
        public void DeserializeId()
        {
            var id = DMTId.NewId();
            var id2 = SerializerHelper.DeserializeObject<DMTId>(SerializerHelper.SerializeObject(id));

            Assert.Equal(id, id2);
        }

    }
}
