using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Serialization
{
    [Export(typeof(IContextFactory))]
    public class ContextFactory : IContextFactory
    {
        public IContext CreateContext()
        {
            return new DeserializationContext();
        }
    }
}
