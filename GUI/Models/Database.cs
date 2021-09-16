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
  }
}
