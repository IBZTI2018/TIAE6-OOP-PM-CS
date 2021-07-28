using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new TIAE6Context())
            {
                foreach (var person in ctx.persons.ToList())
                {
                    Console.WriteLine("id: " + person.id + ", firstname: " + person.firstName + ", lastname: " + person.lastName);
                }
            }
        }
    }
}
