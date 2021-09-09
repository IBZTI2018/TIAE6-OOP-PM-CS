using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    [DataContract]
    public class StatusRequest
    {
        [DataMember(Order = 1)]
        public byte ping { get; set; }
    }

    [DataContract]
    public class StatusResponse
    {
        [DataMember(Order = 1)]
        public byte pong { get; set; }
    }

    [ServiceContract]
    public interface IStatusService
    {
        public ValueTask<StatusResponse> getStatus(StatusRequest request);
    }

    public class StatusService : IStatusService
    {
        public ValueTask<StatusResponse> getStatus(StatusRequest request)
        {
            return new ValueTask<StatusResponse>(new StatusResponse { pong = 1 });
        }
    }
}
