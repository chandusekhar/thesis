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
    class UserReader : EntityReaderBase<UserWrapper>
    {
        const string FirstName = "usr_firstname";
        const string LastName = "usr_lastname";

        public UserReader(string dir)
            : base(dir)
        {

        }

        protected override UserWrapper CreateEntity(CsvReader reader)
        {
            var firstname = reader.GetField(FirstName);
            var lastname = reader.GetField(LastName);

            return new UserWrapper { FirstName = firstname, LastName = lastname };
        }

        protected override string GetFilename()
        {
            return "users.csv";
        }

        protected override string GetIdFieldName()
        {
            return "usr_id";
        }
    }
}
