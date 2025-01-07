using AutoFixture;
using Moq;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstApplication.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Xunit;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace MyFirstApplicationTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personGetterService;
        private readonly IPersonsAdderService _personAdderService;
        private readonly IPersonsUpdaterService _personUpdaterService;
        private readonly IPersonsDeleterService _personDeleterService;
        private readonly IPersonsSorterService _personSorterService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsGetterService> _personGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personAdderServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personSorterServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly Fixture _fixture;

        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();
            _personGetterServiceMock = new Mock<IPersonsGetterService>();
            _personAdderServiceMock = new Mock<IPersonsAdderService>();
            _personSorterServiceMock = new Mock<IPersonsSorterService>();
            _personDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personUpdaterServiceMock = new Mock<IPersonsUpdaterService>();

            _personGetterService = _personGetterServiceMock.Object;
            _personAdderService = _personAdderServiceMock.Object;
            _personSorterService = _personSorterServiceMock.Object;
            _personDeleterService = _personDeleterServiceMock.Object;
            _personUpdaterService = _personUpdaterServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
           _logger = _loggerMock.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arange
            List<PersonResponse> person_response_list = _fixture
                .Create<List<PersonResponse>>();

            PersonsController personsController = new 
                PersonsController(_personGetterService, _countriesService, 
                _logger, _personSorterService, _personAdderService,
                _personDeleterService, _personUpdaterService);

            _personGetterServiceMock.Setup(temp => temp
                .GetFilteredPersons(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(person_response_list);

            _personSorterServiceMock.Setup(temp => temp
               .GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
               .ReturnsAsync(person_response_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<string>(),
                _fixture.Create<SortOrderOptions>() );

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().Be(person_response_list);

        }
        #endregion

        #region Create
        
        #endregion
    }
}
