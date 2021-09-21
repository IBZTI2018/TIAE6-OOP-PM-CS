using System.Collections.Generic;
using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using Shared.Contracts;
using System.Threading.Tasks;
using Shared.Models;

namespace GUI.Models {
  /// <summary>
  /// Database service model.
  /// 
  /// This exposes all data provided by the database service.
  /// </summary>
  class DatabaseModel : GrpcModel, ModelWithServiceStatus {
    private GrpcChannel channel;

    public DatabaseModel()
    {
      this.channel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT);
    }

    /// <summary>
    /// Check whether or not the service is running.
    /// </summary>
    /// <returns>A boolean indicator of wheter or not the service is running</returns>
    public async Task<bool> serviceIsRunning()
    {
        try
        {
            IStatusService statusService = this.channel.CreateGrpcService<IStatusService>();
            StatusResponse response = await statusService.getStatus(new StatusRequest { ping = 1 });
            return response.pong == 1;
        } catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get a list of all inferrence rules
    /// </summary>
    /// <returns>An awaitable task with a list of inference rules</returns>
    public async Task<List<InferenceRule>> getAllInferenceRules()
    {
      IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
      InferenceRulesResponse inferenceRules = await ruleService.getInferenceRules(new EmptyRequest());
      return inferenceRules.rules;
    }

    /// <summary>
    /// Get a list of all evaluation rules
    /// </summary>
    /// <returns>An awaitable task with a list of evaluation rules</returns>
    public async Task<List<EvaluationRule>> getAllEvaluationRules()
    {
      IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
      EvaluationRulesResponse evaluationRules = await ruleService.getEvaluationRules(new EmptyRequest());
      return evaluationRules.rules;
    }

    /// <summary>
    /// Get a list of all persons
    /// </summary>
    /// <returns>An awaitable task with a list of persons</returns>
    public async Task<List<Person>> getAllPersons()
    {
        IPersonService personService = this.channel.CreateGrpcService<IPersonService>();
        PersonListResponse response = await personService.getPersonAll(new EmptyRequest());
        return response.personList;
    }

    /// <summary>
    /// Get all tax declarations
    /// </summary>
    /// <returns>An awaitable task with a list of tax declarations</returns>
    public async Task<List<TaxDeclaration>> getAllTaxDeclarations()
    {
        ITaxDeclarationService taxDeclarationService = this.channel.CreateGrpcService<ITaxDeclarationService>();
        TaxDeclarationListResponse response = await taxDeclarationService.getAllTaxDeclarations(new EmptyRequest());
        return response.declarationList;
    }

    /// <summary>
    /// Persist a new inference rule in the database
    /// </summary>
    /// <param name="rule">The rule to persist</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> saveNewInferenceRule(InferenceRule argRule)
    {
        IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
        BoolResponse response = await ruleService.saveNewInferenceRule(new InferenceRuleRequest { rule = argRule });
        return response.success;
    }

    /// <summary>
    /// Persist a new evaluation rule in the database
    /// </summary>
    /// <param name="rule">The rule to persist</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> saveNewEvaluationRule(EvaluationRule argRule)
    {
        IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
        BoolResponse response = await ruleService.saveNewEvaluationRule(new EvaluationRuleRequest { rule = argRule });
        return response.success;
    }

    /// <summary>
    /// Toggle the active flag of a rule
    /// </summary>
    /// <param name="rule">The rule to toggle</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> toggleActiveRule(Rule argRule)
    {
        IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
        BoolResponse response = await ruleService.toggleActiveRule(new RuleRequest { rule = argRule });
        return response.success;
    }

    /// <summary>
    /// Create a new tax delcaration
    /// 
    /// This is a dummy function. Eventually, this would be done via API.
    /// </summary>
    /// <param name="income">The income of the person</param>
    /// <param name="deductions">The given deductions of a person</param>
    /// <param name="year">The year for which de declaration is filed</param>
    /// <param name="personId">The id of the person to file for</param>
    /// <param name="capital">The capital of the person</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> createNewTaxDeclaration(decimal argIncome, decimal argDeductions, int argYear, int argPersonId, decimal argCapital)
    {
        ITaxDeclarationService taxDeclarationService = this.channel.CreateGrpcService<ITaxDeclarationService>();
        BoolResponse response = await taxDeclarationService.createNewTaxDeclaration(new NewTaxDeclarationRequest {
            income = argIncome,
            deductions = argDeductions,
            year = argYear,
            personId = argPersonId,
            capital = argCapital
        });
        return response.success;
    }
  }
}
