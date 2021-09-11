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

namespace Inferenzmotor {
  class Program {
        public static int INFERENCE_MOTOR_PORT = 9002;
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            /**Example GRPC Client Call START**/
            using (var http = GrpcChannel.ForAddress("http://localhost:" + Shared.Ports.DB_PORT))
            {
                ITaxInformationService taxInformationService = http.CreateGrpcService<ITaxInformationService>();
                // GET
                TaxInformationResponse response = await taxInformationService.getNonInferredWork(new EmptyRequest());

                // PUT
                //response.taxInformation.thisYear.income = 999999;
                //response.taxInformation.thisYear.inferred = false;
                //BoolResponse putResponse = await taxInformationService.putTaxData(new YearlyTaxDataRequest { taxData = response.taxInformation.thisYear });

                IRuleService ruleService = http.CreateGrpcService<IRuleService>();
                InferenceRulesResponse ruleList = await ruleService.getInferenceRules(new EmptyRequest());
                Console.WriteLine("Count Inference Rules: " + ruleList.rules.Count.ToString());

            }
            /**Example GRPC Client Call END**/

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
              options.ListenLocalhost(Shared.Ports.INFERENCE_PORT, listenOptions => {
                  listenOptions.Protocols = HttpProtocols.Http2;
              });
          })
          .UseStartup<Startup>();
    }
}
