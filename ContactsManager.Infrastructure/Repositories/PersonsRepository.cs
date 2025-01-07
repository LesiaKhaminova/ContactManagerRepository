using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using RepositiruContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;

        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
            await  _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personId) );
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("Get all persons of the personsRepository");
            return await _db.Persons
                .Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("Get filtered persons of the personsRepository");
            return await _db.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country")
                .FirstOrDefaultAsync(temp => temp.PersonId == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.Include("Country")
                .FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

            if (matchingPerson == null) return person;

            matchingPerson.FirstName = person.FirstName;
            matchingPerson.LastName = person.LastName;
            matchingPerson.Email = person.Email;
            matchingPerson.Gender = person.Gender;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
            matchingPerson.Adress = person.Adress;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.DateOfBirth = person.DateOfBirth;

            await _db.SaveChangesAsync();
            return matchingPerson;
        }
    }
}
