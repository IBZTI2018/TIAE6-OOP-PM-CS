using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public ValueTask<BoolResponse> toggleActiveRule(RuleRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                request.rule.active = !request.rule.active;
                if (request.rule.GetType() == typeof(InferenceRule))
                {
                    ctx.inferenceRules.Attach((InferenceRule)request.rule).Property(x => x.active).IsModified = true;
                }

                if (request.rule.GetType() == typeof(EvaluationRule))
                {
                    ctx.evaluationRules.Attach((EvaluationRule)request.rule).Property(x => x.active).IsModified = true;
                }

                ctx.SaveChanges();
                BoolResponse response = new BoolResponse { success = true };
                return new ValueTask<BoolResponse>(response);
            }
        }

        public ValueTask<BoolResponse> saveNewInferenceRule(InferenceRuleRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                ctx.inferenceRules.Add(request.rule);
                ctx.SaveChanges();
                BoolResponse response = new BoolResponse { success = true };
                return new ValueTask<BoolResponse>(response);
            }
        }

        public ValueTask<BoolResponse> saveNewEvaluationRule(EvaluationRuleRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                ctx.evaluationRules.Add(request.rule);
                ctx.SaveChanges();
                BoolResponse response = new BoolResponse { success = true };
                return new ValueTask<BoolResponse>(response);
            }
        }
    }
}
