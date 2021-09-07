using System.Runtime.Serialization;

namespace Steuerberechner.Contracts
{
    [DataContract]
    public class IDRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }
}
