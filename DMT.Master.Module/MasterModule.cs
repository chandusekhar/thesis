using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;

namespace DMT.Master.Module
{
    public sealed class MasterModule
    {
        public void Start()
        {
            CompositionService.Instance.Initialize();
        }
    }
}
