using Entities;
using Microsoft.EntityFrameworkCore;
using RepositiruContracts;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();   
        }

        public async Task<Country?> GetCountryById(Guid id)
        {
          return await  _db.Countries
                .FirstOrDefaultAsync(temp => temp.CountryId == id);    
        }

        public async Task<Country?> GetCountryByName(string name)
        {
            return await _db.Countries
                .FirstOrDefaultAsync(temp => temp.CountryName == name);
        }
    }
}