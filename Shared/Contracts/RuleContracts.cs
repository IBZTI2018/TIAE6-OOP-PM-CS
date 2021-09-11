using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Contracts
{
    [DataContract]
    public class InferenceRulesResponse
    {
        [DataMember(Order = 1)]
        public List<InferenceRule> rules { get; set; }
    }

    [DataContract]
    public class EvaluationRulesResponse
    {
        [DataMember(Order = 1)]
        public List<EvaluationRule> rules { get; set; }
    }

    [ServiceContract]
    public interface IRuleService
    {
        public ValueTask<InferenceRulesResponse> getInferenceRules(EmptyRequest request);
        public ValueTask<EvaluationRulesResponse> getEvaluationRules(EmptyRequest request);
    }
}
