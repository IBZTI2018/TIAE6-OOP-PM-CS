using System.Threading.Tasks;
using Shared.Contracts;

namespace Steuerberechner.Contracts
{
    public class TaxCalculatorService : ITaxCalculatorService
    {
        public ValueTask<TaxCalculatorResponse> getTaxes(IDRequest request)
        {
            return new ValueTask<TaxCalculatorResponse>(new TaxCalculatorResponse { value = 1 });
        }
    }
}
