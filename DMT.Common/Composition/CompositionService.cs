using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Composition
{
    /// <summary>
    /// Singleton class for all the composition related tasks
    /// </summary>
    public class CompositionService : IDisposable
    {
        private static CompositionService instance;
        public static CompositionService Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new CompositionService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Overrides the default implementation of the CompositionService. 
        /// </summary>
        /// <param name="newDefault"></param>
        public static void OverrideDefault(CompositionService newDefault)
        {
            instance.Dispose();
            instance = newDefault;
        }

        protected CompositionContainer container;

        protected AggregateCatalog aggregateCatalog;

        protected CompositionService()
        {
            this.aggregateCatalog = new AggregateCatalog();
            this.container = new CompositionContainer(aggregateCatalog);
        }

        /// <summary>
        /// Adds a new catalog to the container. This method is chainable.
        /// </summary>
        /// <param name="catalog">The catalog that contains the types annotated with <c>ExportAttribute</c>.</param>
        /// <returns>the <c>CompositionService</c> instance</returns>
        public CompositionService AddCatalog(ComposablePartCatalog catalog)
        {
            this.aggregateCatalog.Catalogs.Add(catalog);
            return this;
        }

        /// <summary>
        /// Adds a new directory (which cointains assemblies) to the container. This method is chainable.
        /// </summary>
        /// <param name="catalog">The catalog that contains the types annotated with <c>ExportAttribute</c>.</param>
        /// <returns>the <c>CompositionService</c> instance</returns>
        public CompositionService AddDirectory(string path)
        {
            return this.AddCatalog(new DirectoryCatalog(path));
        }

        /// <summary>
        /// Adds a single assembly to the container. This method is chainable.
        /// </summary>
        /// <param name="catalog">The catalog that contains the types annotated with <c>ExportAttribute</c>.</param>
        /// <returns>the <c>CompositionService</c> instance</returns>
        public CompositionService AddAssembly(Assembly assembly)
        {
            return this.AddCatalog(new AssemblyCatalog(assembly));
        }

        /// <summary>
        /// Satisfies imports on the target object. It does not allow automatic recomposition.
        /// </summary>
        /// <param name="target">the object in which the imports should be satisfied.</param>
        public void Inject(object target)
        {
            this.Inject(target, false);
        }

        /// <summary>
        /// Satisfies imports on the target object.
        /// </summary>
        /// <param name="target">the object in which the imports should be satisfied.</param>
        /// <param name="allowRecomposition">if true it allows automatic recomposition</param>
        public void Inject(object target, bool allowRecomposition)
        {
            if (allowRecomposition)
            {
                this.container.ComposeParts(target);
            }
            else
            {
                this.container.SatisfyImportsOnce(target);
            }
        }

        /// <summary>
        /// The exported value for type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetExport<T>()
        {
            return this.container.GetExportedValue<T>();
        }

        #region IDisposable

        public void Dispose()
        {
            container.Dispose();
        } 

        #endregion
    }
}
