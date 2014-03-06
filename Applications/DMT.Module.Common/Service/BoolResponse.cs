using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;

namespace DMT.Module.Common.Service
{
    public class BoolResponse : XmlRouteResponseBase<bool>
    {
        public override bool Result { get; set; }

        public BoolResponse()
        {
            this.Result = false;
        }

        public BoolResponse(bool value)
        {
            this.Success = true;
            this.Result = value;
        }
    }
}
