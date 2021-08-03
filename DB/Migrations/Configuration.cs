namespace DB.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DB.TIAE6Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DB.TIAE6Context";
        }

        protected override void Seed(DB.TIAE6Context context)
        {

            context.municipalities.Add(new Municipality() {
                name = "Sitten"
            });
            context.SaveChanges();

            context.streets.Add(new Street() {
                name = "Bahnhofstrasse",
                postalCode = 1950,
                municipality = context.municipalities.FirstOrDefault(x => x.name == "Sitten")
            });
            context.SaveChanges();

            context.persons.Add(new Person() {
                firstName = "André",
                lastName = "Glatzl",
                street = context.streets.FirstOrDefault(x => x.name == "Bahnhofstrasse"),
                streetNumber = "1A"
            });

            context.persons.Add(new Person()
            {
                firstName = "Sven",
                lastName = "Gehring",
                street = context.streets.FirstOrDefault(x => x.name == "Bahnhofstrasse"),
                streetNumber = "1B"
            });

            base.Seed(context);
        }
    }
}
