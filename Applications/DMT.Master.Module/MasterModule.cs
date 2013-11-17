using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using NLog;

namespace DMT.Master.Module
{
    public sealed class MasterModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Import]
        IEntityFactory factory;

        public void Start()
        {
            logger.Info("Master module started.");

            CompositionService.Instance.Initialize();
            CompositionService.Instance.InjectOnce(this);

            System.Diagnostics.Debug.Assert(factory != null);
        }
    }
}
