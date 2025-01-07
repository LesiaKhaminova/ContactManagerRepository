using Entities;

namespace RepositiruContracts
{
    /// <summary>
    /// represents data access logic for managing Person entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">country object to add</param>
        /// <returns>returns a country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All countries in the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns country object based on the country id ; otherwise returns null
        /// </summary>
        /// <param name="id">Country id to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryById(Guid id);

        /// <summary>
        /// Returns the country object based on the country name
        /// </summary>
        /// <param name="name">country name to search</param>
        /// <returns>returns matching country or null</returns>
        Task<Country?> GetCountryByName(string name);
    }
}