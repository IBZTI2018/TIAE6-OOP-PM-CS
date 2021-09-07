using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Steuerberechner.Contracts
{
    [DataContract]
    public class TaxCalculatorResponse
    {
        [DataMember(Order = 1)]
        public int value { get; set; }
    }

    [ServiceContract(Name = "Steuerberechner.TaxCalculatorService")]
    public interface ITaxCalculatorService
    {
        public ValueTask<TaxCalculatorResponse> getTaxes(IDRequest request);
    }

    public class TaxCalculatorService : ITaxCalculatorService
    {
        public ValueTask<TaxCalculatorResponse> getTaxes(IDRequest request)
        {
            return new ValueTask<TaxCalculatorResponse>(new TaxCalculatorResponse { value = 1 });
        }
    }
}
