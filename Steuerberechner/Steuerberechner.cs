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

namespace Steuerberechner {
  /// <summary>
  /// Tax calculator worker
  /// </summary>
  class Steuerberechner {
    public static Steuerberechner shared = new Steuerberechner();

    private List<Rule> rules;

    /// <summary>
    /// Worker function to be started in a worker thread
    /// </summary>
    public static async void worker()
    {
      // Load all rules on startup
      bool canStart;

      do
      {
        try
        {
          await Steuerberechner.shared.loadRulesFromDatabase();
          canStart = true;
        }
        catch
        {
          Console.WriteLine("Failed to load rules, retrying...");
          canStart = false;
        }
        Thread.Sleep(5000);
      } while (!canStart);

      while (true)
      {
        // Attempt to load a dataset from the database
        Console.WriteLine("Fetching new data from database...");
        TaxInformation newInformation = null;

        try
        {
          using (var http = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT))
          {
            ITaxInformationService taxInformationService = http.CreateGrpcService<ITaxInformationService>();
            TaxInformationResponse response = await taxInformationService.getNonCalculatedWork(new EmptyRequest());
            newInformation = response.taxInformation;
          }
        } catch {
          Console.WriteLine("Failed to fetch data from database");
        }

        // If no new information is found, we just wait a bit and try again
        if (newInformation == null || (newInformation.lastYear == null && newInformation.thisYear == null))
        {
          Console.WriteLine("No new data found, waiting and retrying...");
          Thread.Sleep(10000);
          continue;
        }

        // Otherwise, we process the data
        Console.WriteLine("Found new information, evaluating... (id:" + newInformation.thisYear.id + ")");
        TaxInformation evaluatedInformation = Steuerberechner.shared.evaluateDataset(newInformation);
        await Steuerberechner.shared.putEvaluatedTaxInformation(evaluatedInformation);
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
        EvaluationRulesResponse ruleList = await ruleService.getEvaluationRules(new EmptyRequest());
        this.rules = ruleList.rules.ConvertAll(x => (Rule)x);

        Console.WriteLine("Successfully loaded rules from database (" + ruleList.rules.Count.ToString() + ")");
      }
    }

    /// <summary>
    /// Evaluate a dataset based on the stored rules
    /// </summary>
    /// <param name="input">Tax information of a person to evaluate</param>
    /// <returns>Evaluated tax information of a person</returns>
    public TaxInformation evaluateDataset(TaxInformation input)
    {
      RuleData result = RuleTraversal.traverseRuleTree(this.rules, new RuleData(input.toVariableMap()));
      TaxInformation output = TaxInformation.fromVariableMap(result.data);
      output.thisYear.calculated = true;
      output.thisYear.id = input.thisYear.id;
      output.lastYear.id = input.lastYear.id;
      output.id = input.id;
      return output;
    }

    /// <summary>
    /// Persist evaluated tax information in the database.
    /// </summary>
    public async Task putEvaluatedTaxInformation(TaxInformation input)
    {
      Console.WriteLine("Storing evaluated tax information in database...");
      using (var http = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT))
      {
        ITaxInformationService taxInformationService = http.CreateGrpcService<ITaxInformationService>();
        BoolResponse putResponse = await taxInformationService.putTaxData(new YearlyTaxDataRequest { taxData = input.thisYear });

        if (!putResponse.success)
        {
          throw new Exception("Failed to store evaluated data in database");
        }
      }
    }
  }
}
