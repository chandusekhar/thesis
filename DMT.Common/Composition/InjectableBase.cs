using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Composition
{
    /// <summary>
    /// Base for classes that has satisfiable imports.
    /// It satisfies the imports automatically on instatiation in dev and production environments.
    /// </summary>
    public abstract class InjectableBase
    {
        public InjectableBase()
        {
            if (Configuration.GetEnvironment() != Environment.TEST)
            {
                CompositionService.Default.Inject(this);
            }
        }
    }
}
