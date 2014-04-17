using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Matcher.Local.Diagnostics
{
    class MatcherMetric : IDisposable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly object padlock = new object();
        private static MatcherMetric _default;

        public static MatcherMetric Deafult
        {
            get
            {
                if (_default == null)
                {
                    lock (padlock)
                    {
                        if (_default == null)
                        {
                            _default = new MatcherMetric();
                        }
                    }
                }

                return _default;
            }
        }

        private long sumWaitTimeMs;
        private Stopwatch stopWatch;
        private long elapsed;

        public double SumWaitTimeInSec
        {
            get { return sumWaitTimeMs / 1000d; }
        }
        
        public static IDisposable RegisterWaitTime(string name)
        {
            return new WaitTimer(name, Deafult);
        }

        public IDisposable Start()
        {
            if (stopWatch == null || !stopWatch.IsRunning)
            {
                stopWatch = Stopwatch.StartNew();
            }
            else
            {
                throw new InvalidOperationException("Time tracking has already been started.");
            }

            return this;
        }

        public void LogResults()
        {
            logger.Info("Sum wait time for remote matches: {0:f} s", SumWaitTimeInSec);
            logger.Info("Elapsed time for the whole matching: {0:f} s", elapsed / 1000d);
            logger.Info("{0} % of the time was spent on waiting for remote matches to finish.", ((double)sumWaitTimeMs / elapsed) * 100d);
        }

        void IDisposable.Dispose()
        {
            if (stopWatch == null || !stopWatch.IsRunning)
            {
                throw new InvalidOperationException("Time tracking has not been started yet.");
            }

            stopWatch.Stop();
            elapsed = stopWatch.ElapsedMilliseconds;
        }

        private void AddWaitTime(long time)
        {
            lock (this)
            {
                sumWaitTimeMs += time;
            }
        }

        private class WaitTimer : IDisposable
        {
            private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

            private MatcherMetric metric;
            private Stopwatch sw;
            private string name;

            public WaitTimer(string name, MatcherMetric metric)
            {
                this.metric = metric;
                this.name = name;
                this.sw = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                sw.Stop();
                logger.Trace("{0} finished in {1} ms", name, sw.ElapsedMilliseconds);
                metric.AddWaitTime(sw.ElapsedMilliseconds);
            }
        }
    }
}
