using System;
using System.Collections.Generic;
using System.Text;
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

    public async Task reloadRules()
    {
      IInferenceService service = this.channel.CreateGrpcService<IInferenceService>();
      BoolResponse response = await service.reloadRules(new EmptyRequest());
    }
  }
}
