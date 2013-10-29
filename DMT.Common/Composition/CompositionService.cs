﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
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
        public const string DefaultCatalogPathKey = "default";
        public const string ExtensionCatalogPathKey = "extension";
        public const string ThirdPartyCatalogPathKey = "third-party";

        private static CompositionService instance;
        public static CompositionService Instance
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
        /// Path of the extension DLLs such as DMT.Core.Partition.
        /// </summary>
        private string extensionsPath;
        /// <summary>
        /// Path of the third party impelementations. If present this is the prefered one.
        /// </summary>
        private string thirdPartyPath;

        private CompositionService() { }

        #region public methods

        /// <summary>
        /// Initializes the composition container with two paths. Dlls on the defaults
        /// path used as a fallback for import satisfaction. All extension dll should be placed
        /// in the extension directory.
        /// 
        /// <para>This method uses the value of 'default-catalog' and 'ext-catalog' key in appsettings.</para>
        /// </summary>
        public void Initialize()
        {
            var def = ConfigurationManager.AppSettings[CompositionService.DefaultCatalogPathKey];
            var ext = ConfigurationManager.AppSettings[CompositionService.ExtensionCatalogPathKey];
            var tp = ConfigurationManager.AppSettings[CompositionService.ThirdPartyCatalogPathKey];

            this.Initialize(def, ext, tp);
        }

        /// <summary>
        /// Initializes the composition container with two paths. Dlls on the defaults
        /// path used as a fallback for import satisfaction. All extension dll should be placed
        /// in the extension directory.
        /// </summary>
        /// <param name="defaultsPath">path of the assemblies containing the default implementation</param>
        /// <param name="extensionsPath">path of the assemblies containing the extension implementation</param>
        /// <param name="thirdPartyPath">path of the assemblies containing the third party implementation</param>
        public void Initialize(string defaultsPath, string extensionsPath, string thirdPartyPath)
        {
            this.defaultsPath = defaultsPath;
            this.extensionsPath = extensionsPath;
            this.thirdPartyPath = thirdPartyPath;
            InitializeContainer();
        }

        /// <summary>
        /// Satisfies imports on the target object. It does not allow automatic recomposition.
        /// </summary>
        /// <param name="target">the object in which the imports should be satisfied.</param>
        public void InjectOnce(object target)
        {
            this.container.SatisfyImportsOnce(target);
        }

        /// <summary>
        /// Satisfies imports on the target object. It does allow automatic recomposition if
        /// the composition container changes. It must be allowed on the <c>Import</c> attribute too.
        /// </summary>
        /// <param name="target">the object in which the imports should be satisfied.</param>
        public void Inject(object target)
        {
                this.container.ComposeParts(target);
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

        #endregion

        private void InitializeContainer()
        {
            if (this.container != null)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.defaultsPath) || string.IsNullOrEmpty(this.extensionsPath) || string.IsNullOrEmpty(this.thirdPartyPath))
            {
                throw new InvalidOperationException(
                    "Some DLL paths has not been set up. " +
                    "Put the appropriate values in your App.config or use the Initialize(String, String, String) method to initialize the composotion service.");
            }

            var thirdPartyCatalog = new DirectoryCatalog(this.thirdPartyPath);
            var defaultCatalogEP = new CatalogExportProvider(new DirectoryCatalog(this.defaultsPath));
            var extensionCatalogEP = new CatalogExportProvider(new DirectoryCatalog(this.extensionsPath));

            this.container = new CompositionContainer(thirdPartyCatalog, extensionCatalogEP, defaultCatalogEP);
            defaultCatalogEP.SourceProvider = this.container;
        }
    }
}
