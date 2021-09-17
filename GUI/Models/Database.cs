using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using Shared.Contracts;
using System.Threading.Tasks;
using Shared.Models;

namespace GUI.Models {
  class DatabaseModel : GrpcModel, ModelWithServiceStatus {
    private GrpcChannel channel;

    public DatabaseModel()
    {
      this.channel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT);
    }

    public async Task<bool> serviceIsRunning()
    {
      IStatusService statusService = this.channel.CreateGrpcService<IStatusService>();
      StatusResponse response = await statusService.getStatus(new StatusRequest { ping = 1 });
      return response.pong == 1;
    }

    public async Task<List<InferenceRule>> getAllInferenceRules()
    {
      IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
      InferenceRulesResponse inferenceRules = await ruleService.getInferenceRules(new EmptyRequest());
      return inferenceRules.rules;
    }

    public async Task<List<EvaluationRule>> getAllEvaluationRules()
    {
      IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
      EvaluationRulesResponse evaluationRules = await ruleService.getEvaluationRules(new EmptyRequest());
      return evaluationRules.rules;
    }

    public async Task<List<Person>> getAllPersons()
    {
        IPersonService personService = this.channel.CreateGrpcService<IPersonService>();
        PersonListResponse response = await personService.getPersonAll(new EmptyRequest());
        return response.personList;
    }

    public async Task<List<TaxDeclaration>> getAllTaxDeclarations()
    {
        ITaxDeclarationService taxDeclarationService = this.channel.CreateGrpcService<ITaxDeclarationService>();
        TaxDeclarationListResponse response = await taxDeclarationService.getAllTaxDeclarations(new EmptyRequest());
        return response.declarationList;
    }

    public async Task<bool> saveNewInferenceRule(InferenceRule argRule)
    {
        IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
        BoolResponse response = await ruleService.saveNewInferenceRule(new InferenceRuleRequest { rule = argRule });
        return response.success;
    }

    public async Task<bool> saveNewEvaluationRule(EvaluationRule argRule)
    {
        IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
        BoolResponse response = await ruleService.saveNewEvaluationRule(new EvaluationRuleRequest { rule = argRule });
        return response.success;
    }

    public async Task<bool> toggleActiveRule(Rule argRule)
    {
        IRuleService ruleService = this.channel.CreateGrpcService<IRuleService>();
        BoolResponse response = await ruleService.toggleActiveRule(new RuleRequest { rule = argRule });
        return response.success;
    }

    public async Task<bool> createNewTaxDeckaration(decimal argIncome, decimal argDeductions, decimal argTaxDue, int argYear, int argPersonId)
    {
        ITaxDeclarationService taxDeclarationService = this.channel.CreateGrpcService<ITaxDeclarationService>();
        BoolResponse response = await taxDeclarationService.createNewTaxDeckaration(new NewTaxDeclarationRequest {
            income = argIncome,
            deductions = argDeductions,
            taxDue = argTaxDue,
            year = argYear,
            personId = argPersonId
        });
        return response.success;
    }
  }
}
