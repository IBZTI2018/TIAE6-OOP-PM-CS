using System.Runtime.Serialization;

namespace Inferenzmotor.Contracts
{
    [DataContract]
    public class IDRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }
}
