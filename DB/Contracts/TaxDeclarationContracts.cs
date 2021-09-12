using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace DB.Contracts
{
    public class TaxDeclarationService : ITaxDeclarationService
    {
        public ValueTask<TaxDeclarationListResponse> getAllTaxDeclarations(EmptyRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                TaxDeclarationListResponse response = new TaxDeclarationListResponse { 
                    declarationList = ctx.taxDeclarations
                                         .Include("Entries")
                                         .Include("Entries.Attributes")
                                         .ToList() 
                };
                return new ValueTask<TaxDeclarationListResponse>(response);
            }
        }
    }
}
