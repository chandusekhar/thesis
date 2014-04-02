using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Composition
{
    /// <summary>
    /// Singleton class for all the composition related tasks
    /// </summary>
    public sealed class CompositionService : IDisposable
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

        private CompositionContainer container;
        /// <summary>
        /// Path of the default DLLs, such as DMT.Core
        /// </summary>
        private string defaultsPath;
        /// <summary>
        /// Path of the third party impelementations. If present this is the prefered one.
        /// </summary>
        private string pluginPath;
        /// <summary>
        /// Flag for initialized.
        /// </summary>
        private bool initialized = false;

        private CompositionService() { }

        #region public methods

        /// <summary>
        /// Initializes the composition container with two paths. Dlls on the defaults
        /// path used as a fallback for import satisfaction. All extension dll should be placed
        /// in the extension directory.
        /// 
        /// <para>This method uses the value of 'default-catalog' and 'plugin-catalog' key in appsettings.</para>
        /// </summary>
        public void Initialize(params Assembly[] assemblies)
        {
            var def = Configuration.Current.DefaultsFolder;
            var plugin = Configuration.Current.PluginsFolder;

            this.Initialize(def, plugin, assemblies);
        }

        /// <summary>
        /// Initializes the composition container with two paths. Dlls on the defaults
        /// path used as a fallback for import satisfaction. All extension dll should be placed
        /// in the extension directory.
        /// </summary>
        /// <param name="defaultsPath">path of the assemblies containing the default implementation</param>
        /// <param name="extensionsPath">path of the assemblies containing the extension implementation</param>
        /// <param name="pluginPath">path of the assemblies containing the third party implementation</param>
        public void Initialize(string defaultsPath, string pluginPath, Assembly[] assemblies)
        {
            this.defaultsPath = defaultsPath;
            this.pluginPath = pluginPath;
            InitializeContainer(assemblies);
        }

        /// <summary>
        /// Satisfies imports on the target object. It does not allow automatic recomposition.
        /// </summary>
        /// <param name="target">the object in which the imports should be satisfied.</param>
        public void InjectOnce(object target)
        {
            CheckInitialized();
            this.container.SatisfyImportsOnce(target);
        }

        /// <summary>
        /// Satisfies imports on the target object. It does allow automatic recomposition if
        /// the composition container changes. It must be allowed on the <c>Import</c> attribute too.
        /// </summary>
        /// <param name="target">the object in which the imports should be satisfied.</param>
        public void Inject(object target)
        {
            CheckInitialized();
            this.container.ComposeParts(target);
        }

        /// <summary>
        /// The exported value for type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetExport<T>()
        {
            CheckInitialized();
            return this.container.GetExportedValue<T>();
        }

        #region IDisposable

        public void Dispose()
        {
            container.Dispose();
        }

        #endregion

        #endregion

        private void InitializeContainer(Assembly[] assemblies)
        {
            if (this.container != null)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.defaultsPath) || string.IsNullOrEmpty(this.pluginPath))
            {
                throw new InvalidOperationException(
                    "Some DLL paths has not been set up. " +
                    "Put the appropriate values in your App.config or use the Initialize(String, String) method to initialize the composotion service.");
            }

            Directory.CreateDirectory(this.pluginPath);

            var catalog = new AggregateCatalog(assemblies.Select(a => new AssemblyCatalog(a)));
            catalog.Catalogs.Add(new DirectoryCatalog(this.pluginPath));

            var defaultCatalogEP = new CatalogExportProvider(new DirectoryCatalog(this.defaultsPath));

            this.container = new CompositionContainer(catalog, defaultCatalogEP);
            defaultCatalogEP.SourceProvider = this.container;

            this.initialized = true;
        }

        private void CheckInitialized()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("CompositionService has not been initialized yet!");
            }
        }
    }
}
