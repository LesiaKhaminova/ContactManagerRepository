using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositiruContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;
using Xunit;
using Serilog;
using Microsoft.Extensions.Logging;

namespace MyFirstApplicationTests
{
    public class PersonServiceTest
    {
        //private fields
        private readonly IPersonsGetterService _personGetterService;
        private readonly IPersonsAdderService _personAdderService;
        private readonly IPersonsUpdaterService _personUpdaterService;
        private readonly IPersonsDeleterService _personDeleterService;
        private readonly IPersonsSorterService _personSorterService;
        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture _fixture;
        private readonly IPersonsRepository _personRepository;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;

        //constructor

        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personRepository = _personRepositoryMock.Object;
            _outputHelper = testOutputHelper;
            var diagnosticCntextMock = new Mock<IDiagnosticContext>();
            var loggerMockGetter = new Mock<ILogger<PersonsGetterService>>();
            var loggerMockAdder = new Mock<ILogger<PersonsAdderService>>();
            var loggerMockUpdater = new Mock<ILogger<PersonsUpdaterService>>();
            var loggerMockDeleter = new Mock<ILogger<PersonsDeleterService>>();
            var loggerMockSorter = new Mock<ILogger<PersonsSorterService>>();
            _personGetterService = new PersonsGetterService(_personRepository,loggerMockGetter.Object ,diagnosticCntextMock.Object);
            _personAdderService = new PersonsAdderService(_personRepository, loggerMockAdder.Object, diagnosticCntextMock.Object);
            _personUpdaterService = new PersonsUpdaterService(_personRepository, loggerMockUpdater.Object, diagnosticCntextMock.Object);
            _personDeleterService = new PersonsDeleterService(_personRepository, loggerMockDeleter.Object, diagnosticCntextMock.Object);
            _personSorterService = new PersonsSorterService(_personRepository, loggerMockSorter.Object, diagnosticCntextMock.Object);
        }

        #region AddPerson

        [Fact]
        //When supply null value in person add request should throw argumentNullException
        public async Task AddPerson_NullPerson_ToBeArgumrntNullException()
        {
            //Arange
            PersonAddRequest? person_add_request = null;

            //Act
           Func<Task> action = (async()=> await _personAdderService.AddPerson(person_add_request));
           await action.Should().ThrowAsync<ArgumentNullException>();

        }


        [Fact]
        //When supply null value as personName should throw ArgumentException
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            //Arange
            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.FirstName, null as string).Create();

            Person person = person_add_request.ToPerson();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            Func<Task> action = (async() => await _personAdderService.AddPerson(person_add_request));
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        //When supply proper person details, it should insert the person into the persons list 
        //and it should return an object of person as personResponse, with newly generated id
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arange
            PersonAddRequest? person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "example@gmail.com").Create();

            Person person = person_add_request.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
           
            //Act
            PersonResponse person_respons_from_add = await _personAdderService.AddPerson(person_add_request);
            person_response_expected.PersonId= person_respons_from_add.PersonId;

            //Assert
            //Assert.True(person_respons_from_add.PersonId != Guid.Empty);
            person_respons_from_add.PersonId.Should().NotBe(Guid.Empty);
            person_respons_from_add.Should().Be(person_response_expected);
        }

        [Fact]
        // when supply null value as person last name should throw argumentException
        public async Task AddPerson_PersonNullLastName_ToBeArgumentException()
        {
            //Arange
            PersonAddRequest? person_add =  _fixture.Build<PersonAddRequest>()
                .With(temp => temp.LastName, null as string).Create();
            Person person = person_add.ToPerson();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            Func<Task> action = (async() =>await _personAdderService.AddPerson(person_add));
            await action.Should().ThrowAsync<ArgumentException>();
        }
        #endregion 

        #region GetAllPersons

        [Fact]
        //The GetAllPersons should return emty list by default
        public async Task GetAllPersons_ToBeEmptyList()
        {
            var persons_list = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons_list);
            List<PersonResponse> actual_list = await _personGetterService.GetAllPersons();
            //Assert.Equal(expected_list, actual_list);
            actual_list.Should().BeEmpty();
        }

        [Fact]
        //The GetAllPersons should return valid person list as PersonRespponse
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            List<Person> persons_list = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example@gmail.com").Create(),

                 _fixture.Build<Person>()
                 .With(temp => temp.Country, null as Country)
                 .With(temp => temp.Email, "example2@gmail.com").Create(),

                 _fixture.Build<Person>()
                 .With(temp => temp.Country, null as Country)
                 .With(temp => temp.Email, "example3@gmail.com").Create()
            };

            List<PersonResponse> response_list_expected = persons_list
                .Select(temp => temp.ToPersonResponse()).ToList();
          
            //Assert.Equal(actual_response_list, expected_response_list);

            _outputHelper.WriteLine("Expected response list:");
            foreach (PersonResponse person_response in response_list_expected)
            {
                _outputHelper.WriteLine(person_response.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons_list);
            List<PersonResponse> persons_list_actual = await _personGetterService.GetAllPersons();

            _outputHelper.WriteLine("Actual response list:");
            foreach (PersonResponse person_response in persons_list_actual)
            {
                _outputHelper.WriteLine(person_response.ToString());
            }
            persons_list_actual.Should().BeEquivalentTo(response_list_expected);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        //If we supply null id value it should returns null as personResponse
        public async Task GetPersonByPersonId_NullId_ToBeNull()
        {
            Guid? personId = null;
            //Func<Task> action =
            Assert.Null(await _personGetterService.GetPersonByPersonId(personId));

        }

        [Fact]
        //If we supply valid person id, it should return valid person obj as personResponse
        public async Task GetPersonByPersonId_ValidPersonId_ToBeSuccessful()
        {


            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "example@gmail.com")
                .With(temp => temp.Country, null as Country).Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            PersonResponse? person_response_from_get = await _personGetterService.GetPersonByPersonId(person.PersonId);
            //Assert.Equal(person_response, await _personService.GetPersonByPersonId(person_response.PersonId));
            person_response_from_get.Should().Be(person_response_expected);


        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        //if the search text is empty should return all persons
        public async Task GetFilteredPersons_EmptySearchText()
        {
            List<Person> persons_list = new List<Person>() {
                 _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example@gmail.com").Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example2@gmail.com").Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example3@gmail.com").Create()
             };

            List<PersonResponse> expected_persons_list = persons_list
                .Select(person => person.ToPersonResponse()).ToList();

            _outputHelper.WriteLine("Expected response list:");
            foreach (PersonResponse person_response in expected_persons_list)
            {
                _outputHelper.WriteLine(person_response.ToString());
            }

            _personRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons_list);

            List<PersonResponse> actual_response_list = await _personGetterService.GetFilteredPersons(nameof(Person.FirstName), "");

            _outputHelper.WriteLine("Actual response list:");
            foreach (PersonResponse person_response in actual_response_list)
            {
                _outputHelper.WriteLine(person_response.ToString());
            }
          
            // Assert.Equal(actual_response_list, expected_response_list);
            actual_response_list.Should().BeEquivalentTo(expected_persons_list);
        }

        [Fact]
        //Search by name, should return the matching persons with search string
        public async Task GetFilteredPersons_SearchByPearsonName_ToBeSuccessful()
        {
            List<Person> persons_list = new List<Person>() {
                 _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example@gmail.com").Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example2@gmail.com").Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example3@gmail.com").Create()
             };

            List<PersonResponse> expected_persons_list = persons_list
                .Select(person => person.ToPersonResponse()).ToList();

            _outputHelper.WriteLine("Expected response list:");
            foreach (PersonResponse person_response in expected_persons_list)
            {
                _outputHelper.WriteLine(person_response.ToString());
            }

            _personRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons_list);

            List<PersonResponse> actual_response_list = await _personGetterService.GetFilteredPersons(nameof(Person.FirstName), "sa");

            _outputHelper.WriteLine("Actual response list:");
            foreach (PersonResponse person_response in actual_response_list)
            {
                _outputHelper.WriteLine(person_response.ToString());
            }

            // Assert.Equal(actual_response_list, expected_response_list);
            actual_response_list.Should().BeEquivalentTo(expected_persons_list);
        }

        #endregion

        #region SortedPersons

        [Fact]
        //When we sort based on name in Desc, it should return person list in descending on FirstName
        public async Task GetSortedPersons_DESCByName_ToBeSuccessful()
        {
            List<Person> person_list = new List<Person>() {
             _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example@gmail.com").Create(),

             _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example2@gmail.com").Create(),

            _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "example3@gmail.com").Create()
             };
            List<PersonResponse> person_response_list_expected = person_list
                .Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock
                .Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(person_list);

            List<PersonResponse> response_list = await _personGetterService.GetAllPersons();

            List<PersonResponse> actual_response_list = await
                    _personSorterService.GetSortedPersons(response_list,nameof(Person.FirstName),SortOrderOptions.DESC );

            List<PersonResponse> expected_response_list = response_list.OrderByDescending(temp => temp.FirstName).ToList();

            actual_response_list.Should().BeInDescendingOrder(temp => temp.FirstName);


        }

        #endregion
       

        #region UpdatePerson

        [Fact]
        //When we supply null as PersonUpdateRequest it should throw ArgumentNullException
        public async Task UpdatePerson_NullUpdateRequest_ToBeArgumentNullException()
        {
            PersonUpdateRequest? person_update = null;
            Func<Task> action = (async () => await _personUpdaterService.UpdatePerson(person_update));
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        //When we supply invalid person id it should throw argument exception
        public async Task UpdatePerson_InvalidId_ToBeArgumentException()
        {
            PersonUpdateRequest? person_update = new PersonUpdateRequest() {PersonId = Guid.NewGuid() };
            Func<Task> action = (async() => await _personUpdaterService.UpdatePerson(person_update));
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        //When the person name is null it should throw argument exceptions
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.FirstName, null as string)
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create();

            PersonResponse person_response = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response.ToPearsonUpdateRequest();
     
            Func<Task> action =(async() => await _personUpdaterService.UpdatePerson(person_update_request));
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        //When the person name is null it should throw argument exceptions
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .With(temp => temp.Country, null as Country).Create();

            PersonResponse person_response_expected = person.ToPersonResponse();       
            PersonUpdateRequest person_update_request = person_response_expected.ToPearsonUpdateRequest();

            _personRepositoryMock.Setup(temp => temp
            .UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(temp => temp
            .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            PersonResponse person_response_from_update = await _personUpdaterService.UpdatePerson(person_update_request);

            // Assert.Equal(person_response_from_get, person_response_from_update);
            person_response_from_update.Should().Be(person_response_expected);
        }

        #endregion

        #region DeletePerson

        [Fact]
        //if you supply valid person id it should return true
        public async Task DeletePerson_validPersonId_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create();

            _personRepositoryMock.Setup(temp => temp
                .DeletePersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personRepositoryMock.Setup(temp => temp
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            bool actual_result = await _personDeleterService.DeletePerson(person.PersonId);
            //Assert.True(actual_result);
            actual_result.Should().BeTrue();
        }

        [Fact]
        //if you supply invalid person id it should return false
        public async Task DeletePerson_InvalidPersonId()
        {
            bool actual_result = await _personDeleterService.DeletePerson(Guid.NewGuid());
            //Assert.False(actual_result);
            actual_result.Should().BeFalse();
        }
        #endregion
    }
}
