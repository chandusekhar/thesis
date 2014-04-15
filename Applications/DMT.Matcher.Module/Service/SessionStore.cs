using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module.Service
{
    class SessionStore
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static SessionStore @default;
        private static readonly object padlock = new object();

        public static SessionStore Deafult
        {
            get
            {
                if (@default == null)
                {
                    lock (padlock)
                    {
                        if (@default == null)
                        {
                            @default = new SessionStore();
                        }
                    }
                }

                return @default;
            }
        }

        private ConcurrentDictionary<Guid, Session> sessions;

        public Session this[Guid id]
        {
            get { return sessions[id]; }
        }

        public SessionStore()
        {
            sessions = new ConcurrentDictionary<Guid, Session>();
        }

        public Session CreateSession(Guid id)
        {
            var sess = new Session();
            if (!sessions.TryAdd(id, sess))
            {
                logger.Error("Could not add {0} id to session store.");
            }
            return sess;
        }

        public bool DeleteSession(Guid id)
        {
            Session s = null;
            return sessions.TryRemove(id, out s);
        }
    }
}
