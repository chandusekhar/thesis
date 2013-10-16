using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            Id id = Id.FromGuid(g);
            Id id2 = Id.FromGuid(g);

            Assert.True(id.Equals(id2));
        }

        [Fact]
        public void IdEqualityUsingBuiltInFunction()
        {
            Guid g = Guid.NewGuid();
            Id id = Id.FromGuid(g);
            Id id2 = Id.FromGuid(g);

            Assert.True(id.Equals((object)id2));
        }


        [Fact]
        public void IdNotEquiality()
        {
            Assert.False(Id.NewId().Equals(Id.NewId()));
        }

        [Fact]
        public void SerializeId()
        {
            var id = Id.NewId();
            XDocument doc = SerializerHelper.SerializeObject(id);
            Assert.Equal("root", doc.Root.Name);
            Assert.Equal(id, Id.FromGuid(Guid.Parse(doc.Root.Value)));
        }

        [Fact]
        public void DeserializeId()
        {
            var id = Id.NewId();
            var id2 = SerializerHelper.DeserializeObject<Id>(SerializerHelper.SerializeObject(id));

            Assert.Equal(id, id2);
        }

    }
}
