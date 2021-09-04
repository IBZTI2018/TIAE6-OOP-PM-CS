using System;
using Grpc.Core;

namespace DB
{
    public class Program
    {
        public static int DB_PORT = 9001;
        static void Main(string[] args)
        {
            try
            {
                Server server = new Server
                {
                    Services = { PersonService.BindService(new PersonImpl()) },
                    Ports = { new ServerPort("localhost", DB_PORT, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("Database service listening on port " + DB_PORT);
                Console.WriteLine("Press any key to stop the server...");
                Console.ReadKey();
                server.ShutdownAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception encountered: {ex}");
            }
        }
    }
}
