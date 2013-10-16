using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using NLog;

namespace DMT.Core
{
    public struct Id : IId
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Error("Comparing incompatible IId implementations. {0} and {1}", typeof(Id), other.GetType());
                return false;
            }

            Id other2 = (Id)other;

            return this.value.Equals(other2.value);
        }
    }
}
