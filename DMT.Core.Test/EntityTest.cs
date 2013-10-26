using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DMT.Core.Interfaces;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test
{
    public class EntityTest
    {
        class EntityMock : Entity
        {
            public override bool Remove()
            {
                throw new NotSupportedException();
            }
        }

        [Fact]
        public void EntitySerializationHasId()
        {
            EntityMock em = new EntityMock();
            XDocument doc = SerializerHelper.SerializeObject(em);

            Assert.NotEmpty(doc.Descendants(Id.IdTagName));
        }

        [Fact]
        public void EntityDeserializerHasCorrectId()
        {
            var em = new EntityMock();

            var em2 = SerializerHelper.DeserializeObject<EntityMock>(SerializerHelper.SerializeObject(em));
            Assert.Equal(em.Id, em2.Id);
        }

    }
}
