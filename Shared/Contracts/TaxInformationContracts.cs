using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Shared.Structures;

/// <summary>
/// gRPC contract for the API providing tax informations to worker nodes
/// </summary>
namespace Shared.Contracts
{
    [DataContract]
    public class TaxInformationResponse
    {
        [DataMember(Order = 1)]
        public TaxInformation taxInformation { get; set; }
    }

   
    [DataContract]
    public class YearlyTaxDataRequest
    {
        [DataMember(Order = 1)]
        public YearlyTaxData taxData { get; set; }
    }

    [ServiceContract]
    public interface ITaxInformationService
    {
        public ValueTask<TaxInformationResponse> getNonInferredWork(EmptyRequest request);
        public ValueTask<TaxInformationResponse> getNonCalculatedWork(EmptyRequest request);
        public ValueTask<BoolResponse> putTaxData(YearlyTaxDataRequest request);
    }
}
