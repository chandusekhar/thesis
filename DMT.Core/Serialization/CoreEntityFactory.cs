using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Serialization
{
    public class CoreEntityFactory : IEntityFactory
    {
        public Interfaces.IId CreateId()
        {
            return Id.NewId();
        }
    }
}
