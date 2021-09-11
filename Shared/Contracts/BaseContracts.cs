using System.Runtime.Serialization;

namespace Shared.Contracts
{
    [DataContract]
    public class IDRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }

    [DataContract]
    public class EmptyRequest
    {
        [DataMember(Order = 1)]
        public int empty = 1;
    }

    [DataContract]
    public class BoolResponse {
        [DataMember(Order = 1)]
        public bool success;
    }
}
