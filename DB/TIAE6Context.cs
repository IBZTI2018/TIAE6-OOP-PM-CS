using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Configuration;
using Microsoft.Extensions.Configuration;

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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();
            optionsBuilder.UseSqlServer(config["ConnectionString"]);
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
            catch (DbUpdateException e)
            {
                
            }
            return base.SaveChanges();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Data
            modelBuilder.Entity<Municipality>()
            .HasData(
                new Municipality { id = 1, name = "Sitten" }
            );

            modelBuilder.Entity<Street>()
            .HasData(
                new Street
                {
                    id = 1,
                    name = "Bahnhofstrasse",
                    postalCode = 1950,
                    municipalityId = 1
                }
            );

            modelBuilder.Entity<Person>()
            .HasData(
                new Person
                {
                    id = 1,
                    firstName = "André",
                    lastName = "Glatzl",
                    streetId = 1,
                    streetNumber = "1A"
                }
            );

            modelBuilder.Entity<Person>()
            .HasData(
                new Person
                {
                    id = 2,
                    firstName = "Sven",
                    lastName = "Gehring",
                    streetId = 1,
                    streetNumber = "1B"
                }
            );
        }
    }
}