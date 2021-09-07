using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DB.Contracts
{
    [DataContract]
    public class IDRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }
}
