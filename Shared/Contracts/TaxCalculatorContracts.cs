using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.ServiceModel;

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
