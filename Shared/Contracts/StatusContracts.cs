using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

/// <summary>
/// gRPC contract for the API providing information about a service's status
/// </summary>
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
