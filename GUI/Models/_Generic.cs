using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GUI.Models {
  /// <summary>
  /// Generic absract class for a model based on a gRPC channel
  /// </summary>
  abstract class GrpcModel {
    private GrpcChannel channel;

    public void teardown()
    {
      this.channel.Dispose();
    }
  }

  /// <summary>
  /// Generic interface for a model implementing a service status
  /// </summary>
  interface ModelWithServiceStatus {
    public Task<bool> serviceIsRunning();
  }
}
