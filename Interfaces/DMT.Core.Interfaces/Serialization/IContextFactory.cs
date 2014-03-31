using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Serialization
{
    public interface IContextFactory
    {
        IContext CreateContext();
    }
}
