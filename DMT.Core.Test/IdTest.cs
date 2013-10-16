using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void IdNotEquiality()
        {
            Assert.False(Id.NewId().Equals(Id.NewId()));
        }
    }
}
