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

    [ServiceContract]
    public interface IPersonService
    {
        public ValueTask<PersonResponse> getPerson(IDRequest request);
    }
}
