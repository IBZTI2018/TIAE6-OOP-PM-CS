using System.Runtime.Serialization;
using System.Threading.Tasks;
using Shared.Contracts;

namespace Inferenzmotor.Contracts
{
    public class InferenceService : IInferenceService
    {
        public ValueTask<InferenceResponse> getInference(IDRequest request)
        {
            return new ValueTask<InferenceResponse>(new InferenceResponse { value = 1 });
        }

        public async ValueTask<BoolResponse> reloadRules(EmptyRequest request)
        {
            await Inferenzmotor.shared.loadRulesFromDatabase();
            return new BoolResponse { success = true };  
        }
    }
}
