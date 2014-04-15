using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module.Service
{
    class SessionStore
    {
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

        private Dictionary<Guid, Session> sessions;

        public Session this[Guid id]
        {
            get { return sessions[id]; }
        }

        public SessionStore()
        {
            
        }

        public Session CreateSession(Guid id)
        {
            var sess = new Session();
            sessions.Add(id, sess);
            return sess;
        }

        public bool DeleteSession(Guid id)
        {
            return sessions.Remove(id);
        }
    }
}
