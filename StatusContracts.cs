using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    [DataContract]
    public class StatusResponse
    {
        [DataMember(Order = 1)]
        public byte ping { get; set; }
    }

    [ServiceContract(Name = "Shared.StatusService")]
    public interface IStatusService
    {
        public ValueTask<StatusResponse> getStatus(var request);
    }

    public class StatusService : IStatusService
    {
        public ValueTask<StatusResponse> getStatus(var request)
        {
            return new ValueTask<StatusResponse>(new StatusResponse { ping = 1 });
        }
    }
}
