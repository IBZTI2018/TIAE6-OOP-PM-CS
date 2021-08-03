using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace DB
{
    class TIAE6Context : DbContext
    {
        public DbSet<Person> persons { get; set; }
        public DbSet<Municipality> municipalities { get; set; }
        public DbSet<Street> streets { get; set; }
        public DbSet<InferenceRule> inferenceRules { get; set; }
        public DbSet<EvaluationRule> evaluationRules { get; set; }
        public DbSet<TaxDeclaration> taxDeclarations { get; set; }
        public DbSet<TaxDeclarationAttribute> taxDeclarationAttributes { get; set; }
        public DbSet<TaxDeclarationEntry> taxDeclarationEntries { get; set; }

        public TIAE6Context() : base("name=TIAE6ConnectionString")
        {

        }
        public override int SaveChanges()
        {
            try
            {
                var entries = ChangeTracker.Entries().Where(e => e.Entity is IBaseModel && (e.State == EntityState.Added || e.State == EntityState.Modified));

                foreach (var entityEntry in entries)
                {
                    ((IBaseModel)entityEntry.Entity).modifiedAt = DateTime.Now;

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((IBaseModel)entityEntry.Entity).createdAt = DateTime.Now;
                    }
                }

                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }
    }
}
