using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Data.Parser
{
    class UserWrapper : EntityWrapper<Person>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override Person CreateEntity()
        {
            return new Person(factory) { FirstName = FirstName, LastName = LastName };
        }
    }
}
