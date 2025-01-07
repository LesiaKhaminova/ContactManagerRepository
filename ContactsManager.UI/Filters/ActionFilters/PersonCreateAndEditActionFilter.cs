using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyFirstApplication.Controllers;
using ServiceContracts;
using ServiceContracts.DTO;

namespace MyFirstApplication.Filters.ActionFilters
{
    public class PersonCreateAndEditActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonCreateAndEditActionFilter> _logger;  
        public PersonCreateAndEditActionFilter(ICountriesService countriesService, ILogger<PersonCreateAndEditActionFilter> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is PersonsController personController)
            {
                if (!personController.ModelState.IsValid)
                {
                    List<CountryResponse> country_list = await _countriesService.GetAllCountries();
                    personController.ViewBag.Countries = country_list.Select(temp =>
                        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() }
                    );
                    personController.ViewBag.Errors = personController.ModelState.Values
                        .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personController.View(personRequest);
                }
            }
            else
            {
                await next();
            }
            _logger.LogInformation("In after logic of PersonsCreateAndEdit Action filter");
        }
    }
}
