using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.ServiceModel;

/// <summary>
/// gRPC contract for the API provided by the tax calculator node
/// </summary>
namespace Shared.Contracts
{
    [DataContract]
    public class TaxCalculatorResponse
    {
        [DataMember(Order = 1)]
        public int value { get; set; }
    }

    [ServiceContract]
    public interface ITaxCalculatorService
    {
        public ValueTask<TaxCalculatorResponse> getTaxes(IDRequest request);
        public ValueTask<BoolResponse> reloadRules(EmptyRequest request);
    }
}
