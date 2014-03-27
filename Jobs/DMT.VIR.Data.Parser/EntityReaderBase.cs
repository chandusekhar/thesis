using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace DMT.VIR.Data.Parser
{
    abstract class EntityReaderBase<T> where T : EntityWrapper
    {
        protected string dir;
        private bool initialized;

        public EntityReaderBase(string dir)
        {
            this.dir = dir;
            Initialize();
        }

        public Dictionary<int, T> ReadAll()
        {
            Dictionary<int, T> entities = new Dictionary<int, T>();
            using (var reader = CreateReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetField<int>(GetIdFieldName());
                    var entity = CreateEntity(reader);
                    entity.Id = id;
                    entities.Add(id, entity);
                }
            }
            return entities;
        }

        protected CsvReader CreateReader()
        {
            var path = Path.Combine(dir, GetFilename());
            return new CsvReader(new StreamReader(path), new CsvConfiguration { Delimiter = ";", HasHeaderRecord = true });
        }

        protected abstract T CreateEntity(CsvReader reader);

        protected virtual string GetIdFieldName()
        {
            return "id";
        }

        protected virtual void Initialize()
        {
            
        }

        protected abstract string GetFilename();
    }
}
