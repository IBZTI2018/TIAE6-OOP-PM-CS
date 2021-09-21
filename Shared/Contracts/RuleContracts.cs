using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Shared.Models;

/// <summary>
/// gRPC contract for the API providing rules and rule operations to worker nodes and the UI
/// </summary>
namespace Shared.Contracts
{
    [DataContract]
    public class RuleRequest
    {
        [DataMember(Order = 1)]
        public Rule rule { get; set; }
    }

    [DataContract]
    public class InferenceRuleRequest
    {
        [DataMember(Order = 1)]
        public InferenceRule rule { get; set; }
    }

    [DataContract]
    public class EvaluationRuleRequest
    {
        [DataMember(Order = 1)]
        public EvaluationRule rule { get; set; }
    }

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
        public ValueTask<BoolResponse> toggleActiveRule(RuleRequest request);
        public ValueTask<BoolResponse> saveNewInferenceRule(InferenceRuleRequest request);
        public ValueTask<BoolResponse> saveNewEvaluationRule(EvaluationRuleRequest request);
    }
}
