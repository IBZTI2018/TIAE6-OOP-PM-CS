using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace DB
{
    class PersonImpl : PersonService.PersonServiceBase
    {
        public override Task<PersonResponse> GetPerson(PersonRequest request, ServerCallContext context)
        {
            var response = new PersonResponse();
            using (var ctx = new TIAE6Context())
            {
                Person person = ctx.persons.Find(Int32.Parse(request.Id));
                response = new PersonResponse { FirstName = person.firstName, LastName = person.lastName };
            }
            
            return Task.FromResult(response);
        }
    }
}