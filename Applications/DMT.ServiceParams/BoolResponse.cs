using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ServiceParams
{
    public class BoolResponse
    {
        public bool Value { get; set; }

        public BoolResponse()
        {

        }

        public BoolResponse(bool value)
        {
            this.Value = value;
        }
    }
}
