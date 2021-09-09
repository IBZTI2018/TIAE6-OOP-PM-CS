using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DB
{
    public class Program
    {
        static void Main(string[] args)
        {
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
              options.ListenLocalhost(Shared.Ports.DB_PORT, listenOptions => {
                  listenOptions.Protocols = HttpProtocols.Http2;
              });
          })
          .UseStartup<Startup>();
    }
}