using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Shared.Structures;

namespace Shared.Contracts
{
    [DataContract]
    public class TaxInformationResponse
    {
        [DataMember(Order = 1)]
        public TaxInformation taxInformation { get; set; }
    }

    [DataContract]
    public class TaxInformationListResponse
    {
        [DataMember(Order = 1)]
        public List<TaxInformation> taxInformationList { get; set; }
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
        public ValueTask<TaxInformationListResponse> getNonInferredWork(EmptyRequest request);
        public ValueTask<TaxInformationListResponse> getNonCalculatedWork(EmptyRequest request);
        public ValueTask<bool> putInferredTaxData(YearlyTaxData request);
        public ValueTask<bool> putCalculatedTaxData(YearlyTaxData request);
    }
}
