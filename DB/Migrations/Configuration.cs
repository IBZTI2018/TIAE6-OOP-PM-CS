namespace DB.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DB.TIAE6Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DB.TIAE6Context";
        }

        protected override void Seed(DB.TIAE6Context context)
        {
            context.persons.Add(new Person() {
                firstName = "André",
                lastName = "Glatzl"
            });

            context.persons.Add(new Person()
            {
                firstName = "Sven",
                lastName = "Gehring"
            });

            base.Seed(context);
        }
    }
}
