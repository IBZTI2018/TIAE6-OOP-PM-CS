using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Contracts
{
    [DataContract]
    public class PersonResponse
    {
        [DataMember(Order = 1)]
        public Person person { get; set; }
    }

    [DataContract]
    public class PersonListResponse
    {
        [DataMember(Order = 1)]
        public List<Person> personList { get; set; }
    }

    [ServiceContract]
    public interface IPersonService
    {
        public ValueTask<PersonResponse> getPerson(IDRequest request);
        public ValueTask<PersonListResponse> getPersonAll(EmptyRequest request);
    }
}
