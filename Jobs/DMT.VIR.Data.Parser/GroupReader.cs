using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace DMT.VIR.Data.Parser
{
    class GroupReader : EntityReaderBase<GroupWrapper>
    {
        public GroupReader(string dir)
            : base(dir)
        {

        }

        protected override GroupWrapper CreateEntity(CsvReader reader)
        {
            return new GroupWrapper{ Name = reader.GetField("grp_name") };
        }

        protected override string GetFilename()
        {
            return "groups.csv";
        }

        protected override string GetIdFieldName()
        {
            return "grp_id";
        }
    }
}
