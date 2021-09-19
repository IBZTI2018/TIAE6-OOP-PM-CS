using Shared.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    [DataContract]
    public class TaxDeclarationListResponse
    {
        [DataMember(Order = 1)]
        public List<TaxDeclaration> declarationList { get; set; }
    }
    [DataContract]
    public class NewTaxDeclarationRequest
    {
        [DataMember(Order = 1)]
        public decimal income { get; set; }
        [DataMember(Order = 2)]
        public decimal deductions { get; set; }
        [DataMember(Order = 3)]
        public int year { get; set; }
        [DataMember(Order = 4)]
        public decimal capital { get; set; }
        [DataMember(Order = 5)]
        public int personId { get; set; }
    }

    [ServiceContract]
    public interface ITaxDeclarationService
    {
        public ValueTask<TaxDeclarationListResponse> getAllTaxDeclarations(EmptyRequest request);
        public ValueTask<BoolResponse> createNewTaxDeclaration(NewTaxDeclarationRequest request);
    }
}
