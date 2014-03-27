using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Data.Parser
{
    abstract class EntityWrapper<T> : EntityWrapper
    {
        protected VirEntityFactory factory = new VirEntityFactory();
        
        private T entity;
        public T Entity
        {
            get
            {
                if (entity == null)
                {
                    entity = CreateEntity();
                }

                return entity;
            }
        }

        public abstract T CreateEntity();
    }

    abstract class EntityWrapper
    {
        public int Id { get; set; }
    }
}
