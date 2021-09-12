using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Threading.Tasks;
using System.Threading;

namespace Steuerberechner {
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
