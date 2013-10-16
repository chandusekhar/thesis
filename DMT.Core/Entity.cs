using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    public class Entity : IIdentity
    {
        private Id _id;

        public Entity()
        {
            _id = Core.Id.NewId();
        }

        public IId Id
        {
            get { return _id; }
        }
    }
}
