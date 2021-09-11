using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Contracts;
using Shared.Models;

namespace DB.Contracts
{
    public class RuleService : IRuleService
    {
        public ValueTask<InferenceRulesResponse> getInferenceRules(EmptyRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                InferenceRulesResponse response = new InferenceRulesResponse { rules = ctx.inferenceRules.ToList() };
                return new ValueTask<InferenceRulesResponse>(response);
            }
        }
        public ValueTask<EvaluationRulesResponse> getEvaluationRules(EmptyRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                EvaluationRulesResponse response = new EvaluationRulesResponse { rules = ctx.evaluationRules.ToList() };
                return new ValueTask<EvaluationRulesResponse>(response);
            }
        }
    }
}
