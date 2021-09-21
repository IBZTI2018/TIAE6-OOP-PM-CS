using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Shared.Models;
using Shared.Structures;
using Shared.TreeTraversal;
using Grpc.Net.Client;
using Shared.Contracts;
using ProtoBuf.Grpc.Client;

namespace Inferenzmotor {
  /// <summary>
  /// Tax inference worker
  /// </summary>
  class Inferenzmotor {
    public static Inferenzmotor shared = new Inferenzmotor();

    private List<Rule> rules;

    /// <summary>
    /// Worker function to be started in a worker thread
    /// </summary>
    public static async void worker()
    {
      // Load all rules on startup
      await Inferenzmotor.shared.loadRulesFromDatabase();

      while (true)
      {
        // Attempt to load a dataset from the database
        Console.WriteLine("Fetching new data from database...");
        TaxInformation newInformation;
        using (var http = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT))
        {
          ITaxInformationService taxInformationService = http.CreateGrpcService<ITaxInformationService>();
          TaxInformationResponse response = await taxInformationService.getNonInferredWork(new EmptyRequest());
          newInformation = response.taxInformation;
        }

        // If no new information is found, we just wait a bit and try again
        if (newInformation == null || (newInformation.lastYear == null && newInformation.thisYear == null))
        {
          Console.WriteLine("No new data found, waiting and retrying...");
          Thread.Sleep(10000);
          continue;
        }

        // Otherwise, we process the data
        Console.WriteLine("Found new information, inferring... (id:" + newInformation.thisYear.id + ")");
        TaxInformation inferredInformation = Inferenzmotor.shared.inferDataset(newInformation);
        await Inferenzmotor.shared.putInferredTaxInformation(inferredInformation);
        Thread.Sleep(1000);
      }
    }

    /// <summary>
    /// Load all currently active rules from the database
    /// </summary>
    public async Task loadRulesFromDatabase()
    {
      Console.WriteLine("Loading rules from database...");
      using (var http = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT))
      {
        IRuleService ruleService = http.CreateGrpcService<IRuleService>();
        InferenceRulesResponse ruleList = await ruleService.getInferenceRules(new EmptyRequest());
        this.rules = ruleList.rules.ConvertAll(x => (Rule)x);

        Console.WriteLine("Successfully loaded rules from database (" + ruleList.rules.Count.ToString() + ")");
      }
    }

    /// <summary>
    /// Infer a dataset based on the stored rules
    /// </summary>
    /// <param name="input">Tax information of a person to infer</param>
    /// <returns>Inferred tax information of a person</returns>
    public TaxInformation inferDataset(TaxInformation input)
    {
      RuleData result = RuleTraversal.traverseRuleTree(this.rules, new RuleData(input.toVariableMap()));
      TaxInformation output = TaxInformation.fromVariableMap(result.data);
      output.thisYear.inferred = true;
      output.thisYear.id = input.thisYear.id;
      output.lastYear.id = input.lastYear.id;
      output.id = input.id;
      return output;
    }

    /// <summary>
    /// Persist inferred tax information in the database.
    /// </summary>
    public async Task putInferredTaxInformation(TaxInformation input)
    {
      Console.WriteLine("Storing inferred tax information in database...");
      using (var http = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT))
      {
        ITaxInformationService taxInformationService = http.CreateGrpcService<ITaxInformationService>();
        BoolResponse putResponse = await taxInformationService.putTaxData(new YearlyTaxDataRequest { taxData = input.thisYear });

        if (!putResponse.success)
        {
          throw new Exception("Failed to store inferred data in database");
        }
      }
    }
  }
}
