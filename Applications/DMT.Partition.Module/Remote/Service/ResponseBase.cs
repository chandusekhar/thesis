using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote.Service
{
    public abstract class ResponseBase
    {
        public bool Success { get; set; }

        public ResponseBase()
        {
            Success = false;
        }

        public ResponseBase(bool success)
        {
            this.Success = success;
        }
    }
}
