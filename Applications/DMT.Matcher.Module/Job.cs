using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Core.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.Matcher.Module.Exceptions;
using DMT.Matcher.Module.Partitioner;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module
{
    internal class Job : IDisposable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string[] dependencies;
        private IMatcherJob job;
        private bool jobStarted;

        public Job(JobTypeResult result)
        {
            this.dependencies = result.DependencyProvider.GetDependencies();
            AppDomain.CurrentDomain.AssemblyResolve += ResolveDependencies;

            this.job = InstantiateJob(result.JobType);
            job.Done += HandleJobDone;
        }

        public void Start(IModel model, MatchMode mode)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (this.jobStarted)
            {
                throw new JobAlreadyStartedException(string.Format("Job with name {0} has already started.", this.job.Name));
            }
            this.jobStarted = true;

            // start the task on a background thread
            Task.Run(() => this.job.Start(model, mode));

            logger.Info("Matcher job (name: {0}) has been started in {1} mode", this.job.Name, mode);
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveDependencies;
        }

        private void HandleJobDone(object sender, MatcherJobDoneEventArgs e)
        {
            this.jobStarted = false;

            logger.Info("Matcher job (name: {0}) has been finished.", this.job.Name);
            var client = MatcherModule.Instance.CreatePartitionServiceClient();
            client.MarkMatcherDone(MatcherModule.Instance.Id, new MatchFoundRequest { MatchFound = e.HasMatches });
            logger.Debug("Matcher job reported done to partition module.");
        }

        private Assembly ResolveDependencies(object sender, ResolveEventArgs args)
        {
            logger.Debug("Resolving dependency for {0}", args.Name);
            foreach (var dep in this.dependencies)
            {
                if (args.Name.Contains(dep))
                {
                    return LoadDepencdency(dep);
                }
            }

            logger.Warn("Could not resolve dependency for {0}", args.Name);

            return null;
        }

        private Assembly LoadDepencdency(string depName)
        {
            string[] paths = new[]
            {
                string.Concat(Configuration.Current.DefaultsFolder, "/", depName, ".dll"),
                string.Concat(Configuration.Current.PluginsFolder, "/", depName, ".dll"),
            };

            foreach (var path in paths)
            {
                var fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
                if (fi.Exists)
                {
                    return Assembly.LoadFile(fi.FullName);
                }
            }

            logger.Warn("No dll file exists in the additional probing paths.");
            return null;
        }

        private IMatcherJob InstantiateJob(Type jobType)
        {
            var job = (IMatcherJob)Activator.CreateInstance(jobType);
            job.Initialize(new MatcherFrameworkLink());
            logger.Info("Matcher job has been initialized successfully.");

            return job;
        }
    }
}
