using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces.Serialization;
using DMT.Core.Serialization;

namespace DMT.VIR.Data.Serialization
{
    [Export(typeof(IContextFactory))]
    public class VirContextFactory : IContextFactory
    {
        public IContext CreateContext()
        {
            return new Context();
        }
    }
}
