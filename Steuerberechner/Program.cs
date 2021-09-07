using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using DB.Contracts;
using System;
using System.Threading.Tasks;

namespace Steuerberechner
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () => {
                GrpcClientFactory.AllowUnencryptedHttp2 = true;
                using (var http = GrpcChannel.ForAddress("http://localhost:" + DB.Program.DB_PORT))
                {
                    IPersonService personService = http.CreateGrpcService<IPersonService>();
                    PersonResponse response = await personService.getPerson(new IDRequest { id = 1 });
                    Console.WriteLine(response.person.firstName);
                }
            }).GetAwaiter().GetResult();
        }
    }
}
