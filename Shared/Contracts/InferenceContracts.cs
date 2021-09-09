using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace Shared.Contracts
{
    [DataContract]
    public class InferenceResponse
    {
        [DataMember(Order = 1)]
        public int value { get; set; }
    }

    public interface IInferenceService
    {
        public ValueTask<InferenceResponse> getInference(IDRequest request);
    }
}
