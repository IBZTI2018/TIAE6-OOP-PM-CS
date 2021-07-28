using System.Data.Entity;

namespace DB
{ 
    class TIAE6Context : DbContext
    {
        public DbSet<Person> persons { get; set; }
        public TIAE6Context() : base("name=TIAE6ConnectionString")
        {

        }
    }
}
