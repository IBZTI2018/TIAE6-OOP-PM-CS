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

    [ServiceContract]
    public interface ITaxDeclarationService
    {
        public ValueTask<TaxDeclarationListResponse> getAllTaxDeclarations(EmptyRequest request);
    }
}
