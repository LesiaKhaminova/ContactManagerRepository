using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using MyFirstApplication.Filters.ActionFilters;
using MyFirstApplication.Filters.ResultFilters;
using MyFirstApplication.Filters.ResourceFilters;
using MyFirstApplication.Filters.AuthorizationFilters;
using MyFirstApplication.Filters.ExceptionFilters;
using MyFirstApplication.Filters;
using OfficeOpenXml.Style;
using Services;

namespace MyFirstApplication.Controllers
{
    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-key-from-controller", "My-value-from-controller",3 }, Order =2)]
    [ResponseHeaderFilterFactory("My-key-from-controller", "My-value-from-controller", 3 )]
   // [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        public PersonsController(IPersonsGetterService personsGetterService, 
            ICountriesService countriesService, ILogger<PersonsController> logger, 
            IPersonsSorterService personsSorterService, IPersonsAdderService personsAdderService,
            IPersonsDeleterService personsDeleterService, IPersonsUpdaterService personsUpdaterService)
        {
            _personsGetterService = personsGetterService;
            _countriesService = countriesService;
            _logger = logger;
            _personsSorterService = personsSorterService;
            _personsAdderService = personsAdderService;
            _personsDeleterService = personsDeleterService;
            _personsUpdaterService = personsUpdaterService;
        }

        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter))]
        [ResponseHeaderFilterFactory("My-key-from-action", "My-value-from-action", 1 )]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, 
            string sortBy = nameof(PersonResponse.FirstName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of the Persons controller");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrderOptions: {sortOrder}");
            List<PersonResponse> person_response_list = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            List<PersonResponse> sorted_persons_list = await _personsSorterService.GetSortedPersons(person_response_list, sortBy, sortOrder);


            return View(sorted_persons_list);
        }

        //executes when the user clicks on "create person " hyperlink 
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> country_list =await _countriesService.GetAllCountries();
            ViewBag.Countries = country_list.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() }
            );
            
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            await _personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index","Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? current_person=await _personsGetterService.GetPersonByPersonId(personID);
            if(current_person == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = current_person.ToPearsonUpdateRequest();
            List<CountryResponse> country_list =await _countriesService.GetAllCountries();
            ViewBag.Countries = country_list.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() }
            );
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
    
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
           PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personRequest.PersonId);  
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
             PersonResponse personResponseFromUpdate = await _personsUpdaterService.UpdatePerson(personResponse.ToPearsonUpdateRequest());
             return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personID);
            if(personResponse == null) { return RedirectToAction("Index"); }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonResponse personResponse)
        {
            if (personResponse == null) { return RedirectToAction("Index"); }
            await _personsDeleterService.DeletePerson(personResponse.PersonId);
            return RedirectToAction("Index");
        }


        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons_list = await _personsGetterService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons_list, ViewData) { 
            PageMargins = new Rotativa.AspNetCore.Options.Margins() {Top = 20, Right = 20, Bottom = 20, Left = 20 },
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsSCV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonCSV();
            return File(memoryStream, "application/octet-stream","persons.csv");
        }

        [Route("PersonExcel")]
        public async Task<IActionResult> PersonExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","persons.xlsx");
        }
    }
}
