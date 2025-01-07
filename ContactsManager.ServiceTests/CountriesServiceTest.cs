using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;
using Moq;
using OfficeOpenXml.FormulaParsing.Ranges;
using AutoFixture;
using RepositiruContracts;
using FluentAssertions;


namespace MyFirstApplicationTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        //constructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        //When Country request is null should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeNullArgumentException()
        {
            //Arange
            CountryAddRequest? countryAddRequest = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act 
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        //When country name is nuul should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            CountryAddRequest country = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _countriesService.AddCountry(country);
            });
        }

        //When country name is duplicate it should trow argument exception
        [Fact]
        public async Task Add_Country_DuplicateCountryName_ToBeArgumentException()
        {
            CountryAddRequest countryAddRequest1 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();
            CountryAddRequest countryAddRequest2 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();
            Country country = countryAddRequest1.ToCountry();
            _countriesRepositoryMock.Setup(temp => temp
                .GetCountryByName(It.IsAny<string>()))
                .ReturnsAsync(country);
            _countriesRepositoryMock.Setup(temp => temp
                .AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _countriesService.AddCountry(countryAddRequest1);
                await _countriesService.AddCountry(countryAddRequest2);
            });
        }

        //when you supply proper country name it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();
            Country country = countryAddRequest.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp
               .GetCountryByName(It.IsAny<string>()))
               .ReturnsAsync(country);
            _countriesRepositoryMock.Setup(temp => temp
                .AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            CountryResponse actual_response = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse expected_response = country.ToCountryResponse();
            expected_response.CountryId = actual_response.CountryId;

            Assert.True(actual_response.CountryId != Guid.Empty);
            actual_response.Should().Be(expected_response);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        //the list of countries should be empty by default(befor adding any countries)
        public async Task GetAllCountries_EmptyList()
        {
           List<CountryResponse> actual_countries_from_response_list = await _countriesService.GetAllCountries();

           Assert.Empty(actual_countries_from_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> country_requst_list = _fixture.Create<List<CountryAddRequest>>();

            List<CountryResponse> country_response_list = new List<CountryResponse>();

            foreach (CountryAddRequest country_add_request in country_requst_list)
            {
                country_response_list.Add(await _countriesService.AddCountry(country_add_request));
            }

            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            foreach(CountryResponse expected_country in country_response_list)
            {
                Assert.Contains(expected_country, actual_country_response_list);
            }
        }

        #endregion

        #region GetCountryByCountryId

        [Fact]
        //if we supply null as country id it should return null as country respone
        public async Task GetCountryByCountryId_NullCountryId()
        {
            Guid? countryId = null;

            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryId(countryId);

            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        //if we supply a valid country id it should return the matching country details as country response obj
        public async Task GetCountryByCountryId_ValidCountryId()
        {
            CountryAddRequest? country_add_request = new CountryAddRequest() {CountryName="USA" };
            CountryResponse country_response_from_add_request =await _countriesService.AddCountry(country_add_request);

            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country_response_from_add_request.CountryId);

            Assert.Equal(country_response_from_add_request, country_response_from_get);
        }

        #endregion
    }
}
