using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Grpc.Net.Client;
using Shared.Contracts;
using ProtoBuf.Grpc.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Shared.Models;
using Shared.Structures;
using Shared.TreeTraversal;

namespace Steuerberechner {
  class Steuerberechner {
    public static Steuerberechner shared = new Steuerberechner();

    private List<Rule> rules;

    // Worker function to be started in a worker thread
    public static async void worker()
    {
      // Load all rules on startup
      await Steuerberechner.shared.loadRulesFromDatabase();

      while (true)
      {
        // Attempt to load a dataset from the database
        Console.WriteLine("Fetching new data from database...");
        TaxInformation newInformation;
        using (var http = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT))
        {
          ITaxInformationService taxInformationService = http.CreateGrpcService<ITaxInformationService>();
          TaxInformationResponse response = await taxInformationService.getNonCalculatedWork(new EmptyRequest());
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
        Console.WriteLine("Found new information, evaluating... (id:" + newInformation.thisYear.id + ")");
        TaxInformation evaluatedInformation = Steuerberechner.shared.evaluateDataset(newInformation);
        await Steuerberechner.shared.putEvaluatedTaxInformation(evaluatedInformation);
        Thread.Sleep(1000);
      }
    }

    // Load all currently active rules from the database
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

    // Infer a dataset based on the stored rules
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

  class Program {
    static async Task Main(string[] args)
    {
      // Allow unencrypted HTTP/2 connections
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

      // Start inference worker thread
      Thread thread = new Thread(Steuerberechner.worker);
      thread.Start();

      try
      {
        CreateHostBuilder(args).Build().Run();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception encountered: {ex}");
      }

    }

    public static IWebHostBuilder CreateHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
      .ConfigureKestrel(options => {
        options.ListenLocalhost(Shared.Network.TAX_PORT, listenOptions => {
          listenOptions.Protocols = HttpProtocols.Http2;
        });
      })
      .UseStartup<Startup>();
  }
}
