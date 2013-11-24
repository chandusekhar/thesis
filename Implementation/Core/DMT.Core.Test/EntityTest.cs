using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Test.Utils;
using Moq;
using Xunit;

namespace DMT.Core.Test
{
    public class EntityTest
    {
        class EntityMock : Entity
        {
            public EntityMock()
                : this(new CoreEntityFactory())
            {

            }

            public EntityMock(IEntityFactory factory)
                : base(factory)
            {

            }

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

            Assert.NotEmpty(doc.Descendants(DMTId.IdTagName));
        }

        [Fact]
        public void EntityDeserializerHasCorrectId()
        {
            var em = new EntityMock();

            var em2 = SerializerHelper.DeserializeObject<EntityMock>(SerializerHelper.SerializeObject(em));
            Assert.Equal(em.Id, em2.Id);
        }

        [Fact]
        public void EntityEqualityIsBasedOnId()
        {
            IId id = DMTId.NewId();
            var factoryMock = new Mock<IEntityFactory>();
            factoryMock.Setup(f => f.CreateId()).Returns(id);

            var em1 = new EntityMock(factoryMock.Object);
            var em2 = new EntityMock(factoryMock.Object);

            Assert.True(em1.Equals(em2));
        }

        [Fact]
        public void EntityEqualityWhenOtherIsNull()
        {
            var em = new EntityMock();
            Assert.False(em.Equals(null));
        }

        [Fact]
        public void EntityEqualityWhenComparedToSelf()
        {
            EntityMock em = new EntityMock();
            EntityMock other = em;

            Assert.True(em.Equals(em));
            Assert.True(em.Equals(other));
        }
    }
}
