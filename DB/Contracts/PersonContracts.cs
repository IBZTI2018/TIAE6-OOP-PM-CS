using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DB.Contracts
{

    [DataContract]
    public class PersonResponse
    {
        [DataMember(Order = 1)]
        public Person person { get; set; }
    }

    [ServiceContract(Name = "DB.PersonService")]
    public interface IPersonService
    {
        public ValueTask<PersonResponse> getPerson(IDRequest request);
    }

    public class PersonService : IPersonService
    {
        public ValueTask<PersonResponse> getPerson(IDRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                Person person = ctx.persons
                                    .Include(x => x.street)
                                    .Include(x => x.street.municipality)
                                    .FirstOrDefault(x => x.id == request.id);
                var result = new PersonResponse { person = person };
                return new ValueTask<PersonResponse>(result);
            }
        }
    }
}
