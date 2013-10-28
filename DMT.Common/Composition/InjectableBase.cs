using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Composition
{
    /// <summary>
    /// Base for classes that has satisfiable imports.
    /// It satisfies the imports automatically on instatiation in dev and production environments.
    /// </summary>
    public abstract class InjectableBase : IPartImportsSatisfiedNotification
    {
        private Configuration config;

        protected Configuration Config
        {
            get { return config; }
        }

        public InjectableBase()
            : this(Configuration.Current)
        {
        }

        public InjectableBase(Configuration config)
            : this(config, false)
        {
        }

        public InjectableBase(Configuration config, bool allowRecomposition)
        {
            this.config = config;
            if (this.config.Environment != Environment.Test)
            {
                if (allowRecomposition)
                {
                    CompositionService.Instance.Inject(this);
                }
                else
                {
                    CompositionService.Instance.InjectOnce(this);
                }
            }

        }

        /// <summary>
        /// This is called, when all imports that could be satisfied have been satisfied.
        /// </summary>
        public virtual void OnImportsSatisfied() { }
    }
}
