using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;
namespace ServiceContracts
{
    /// <summary>
    /// Represents buisness logic for manipulating Country entitirs
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to  the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns a country object after adding it(including newly generated country id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries
        /// </summary>
        /// <returns>all countries from the list as list of CountriesResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns country obj based on the country id
        /// </summary>
        /// <param name="CountryId">Country id (guid) to search</param>
        /// <returns>Matching country as country response obj</returns>
         Task<CountryResponse?> GetCountryByCountryId(Guid? CountryId);

        /// <summary>
        /// Upload countries from excel file to database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }

}