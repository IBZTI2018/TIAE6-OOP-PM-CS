using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Inferenzmotor.Contracts
{
    [DataContract]
    public class InferenceResponse
    {
        [DataMember(Order = 1)]
        public int value { get; set; }
    }

    [ServiceContract(Name = "Inferenzmotor.InferenceService")]
    public interface IInferenceService
    {
        public ValueTask<InferenceResponse> getInference(IDRequest request);
    }

    public class InferenceService : IInferenceService
    {
        public ValueTask<InferenceResponse> getInference(IDRequest request)
        {
            return new ValueTask<InferenceResponse>(new InferenceResponse { value = 1 });
        }
    }
}
