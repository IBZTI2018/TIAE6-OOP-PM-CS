using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using Shared.Contracts;
using System.Threading.Tasks;

namespace GUI.Models {
  class InferenzmotorModel : GrpcModel, ModelWithServiceStatus {
    private GrpcChannel channel;
  
    public InferenzmotorModel()
    {
      this.channel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.INFERENCE_PORT);
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
    /// Reload the rules that the inference worker is using
    /// </summary>
    /// <returns>An awaitable task</returns>
    public async Task reloadRules()
    {
      IInferenceService service = this.channel.CreateGrpcService<IInferenceService>();
      BoolResponse response = await service.reloadRules(new EmptyRequest());
    }
  }
}
