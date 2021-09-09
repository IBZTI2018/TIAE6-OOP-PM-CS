using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Shared.Contracts;
using Shared.Models;

namespace DB.Contracts
{

    public class PersonService : IPersonService
    {
        public ValueTask<PersonResponse> getPerson(IDRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                Person p = ctx.persons
                                    .Include(x => x.street)
                                    .Include(x => x.street.municipality)
                                    .FirstOrDefault(x => x.id == request.id);
                var result = new PersonResponse { person = p };
                return new ValueTask<PersonResponse>(result);
            }
        }
    }
}
