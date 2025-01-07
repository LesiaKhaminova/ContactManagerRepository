using System;
using System.Collections.Generic;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        public ApplicationDbContext(DbContextOptions options ) :base( options )
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            string countriesJson =  System.IO.File.ReadAllText("countries.json");
            List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            if(countries != null)
            {
                foreach (Country country in countries)
                {
                    modelBuilder.Entity<Country>().HasData(country);
                }
            }

            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
            if(persons != null)
            {
                foreach(Person person in persons)
                {
                    modelBuilder.Entity<Person>().HasData(person);
                }
            }

            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC123");

           /* modelBuilder.Entity<Person>(entity => {
                entity.HasOne<Country>(c => c.Country)
                  .WithMany(p => p.Persons)
                  .HasForeignKey(p => p.CountryId);
            });*/
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
    }
}
