using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

/// <summary>
/// gRPC contract for the API provided by the tax inference node
/// </summary>
namespace Shared.Contracts
{
    [DataContract]
    public class InferenceResponse
    {
        [DataMember(Order = 1)]
        public int value { get; set; }
    }

    [ServiceContract]
    public interface IInferenceService
    {
        public ValueTask<InferenceResponse> getInference(IDRequest request);
        public ValueTask<BoolResponse> reloadRules(EmptyRequest request);
    }
}
