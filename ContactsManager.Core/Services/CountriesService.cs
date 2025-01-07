using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositiruContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(countryAddRequest.CountryName == null)
                throw new ArgumentNullException(nameof(countryAddRequest.CountryName));

            if (await _countriesRepository.GetCountryByName(countryAddRequest.CountryName) != null)
                throw new ArgumentException("Given country name already exist");

            //convert object from countryAddRequest ti Country
            Country country = countryAddRequest.ToCountry();

            //generate id
            country.CountryId = Guid.NewGuid();
            
            //Add new object to the list
            await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse();

        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return  (await _countriesRepository.GetAllCountries())
                .Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? CountryId)
        {
            if (CountryId == null) return null;

            Country? country_from_list = await _countriesRepository.GetCountryById(CountryId.Value);

            return country_from_list == null ? null : country_from_list.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();  
            await formFile.CopyToAsync(memoryStream);

            int CountriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = excelWorksheet.Dimension.Rows;
                for(int row = 2; row < rowCount; row++)
                {
                    string? cellValue = excelWorksheet.Cells[row, 1].Value.ToString();

                    if(!string.IsNullOrEmpty(cellValue))
                    {
                        string countryName = cellValue;

                        if(_countriesRepository.GetCountryByName(countryName) == null)
                        {
                            Country country_to_add = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country_to_add);
                            CountriesInserted++;
                        }
                    }
                }
            }
            return CountriesInserted;
        }
    }
}