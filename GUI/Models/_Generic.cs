using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GUI.Models {
  abstract class GrpcModel {
    private GrpcChannel channel;

    public void teardown()
    {
      this.channel.Dispose();
    }
  }

  interface ModelWithServiceStatus {
    public Task<bool> serviceIsRunning();
  }
}
