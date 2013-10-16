using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    public struct Id : IId
    {
        public static readonly Id Empty = new Id(Guid.Empty);
        
        private Guid value;

        public static Id NewId()
        {
            return new Id(Guid.NewGuid());
        }

        private Id(Guid value)
        {
            this.value = value;
        }

        public bool Equals(IId other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (!(other is Id))
            {
                throw new ArgumentException("Not compatible Ids");
            }

            Id other2 = (Id)other;

            return this.value.Equals(other2.value);
        }
    }
}
