using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using Shared.Contracts;
using System.Threading.Tasks;

namespace GUI.Models {
  class SteuerberechnerModel : GrpcModel, ModelWithServiceStatus {
    private GrpcChannel channel;

    public SteuerberechnerModel()
    {
      this.channel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.TAX_PORT);
    }

    /// <summary>
    /// Check whether or not the service is running.
    /// </summary>
    /// <returns>A boolean indicator of wheter or not the service is running</returns>
    public async Task<bool> serviceIsRunning()
    {
        try
        {
            IStatusService statusService = this.channel.CreateGrpcService<IStatusService>();
            StatusResponse response = await statusService.getStatus(new StatusRequest { ping = 1 });
            return response.pong == 1;
        } catch
        {
            return false;
        }
    }

    /// <summary>
    /// Reload the rules that the evaluation worker is using
    /// </summary>
    /// <returns>An awaitable task</returns>
    public async Task reloadRules()
    {
      ITaxCalculatorService service = this.channel.CreateGrpcService<ITaxCalculatorService>();
      BoolResponse response = await service.reloadRules(new EmptyRequest());
    }
  }
}
