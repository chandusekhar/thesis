using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    [Export(typeof(IModelFactory))]
    public class CoreModelFactory : IModelFactory
    {
        public IModel CreateEmpty()
        {
            return new Model();
        }

        public IModel Create(IEnumerable<INode> nodes)
        {
            return new Model(nodes);
        }
    }
}
