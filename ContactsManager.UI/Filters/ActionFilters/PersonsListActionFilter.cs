using Microsoft.AspNetCore.Mvc.Filters;
using MyFirstApplication.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace MyFirstApplication.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        { _logger = logger; }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //add after logic
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if(parameters != null ) {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["searchBy"] = Convert.ToString(parameters["searchBy"]);
                }          
                if (parameters.ContainsKey("CurrentSearchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }
                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.FirstName);
                }
                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }

            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(PersonResponse.FirstName), "First Name" },
                {nameof(PersonResponse.LastName), "Last Name" },
                {nameof(PersonResponse.Email), "Email" },
                {nameof(PersonResponse.Adress), "Address" },
                {nameof(PersonResponse.CountryName), "Country" },
                {nameof(PersonResponse.Gender), "Gender" }

            };

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //add before logic
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));
            context.HttpContext.Items["arguments"] = context.ActionArguments;

           if(context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if(!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>() { 
                    nameof(PersonResponse.FirstName),
                    nameof(PersonResponse.LastName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.CountryId),
                    nameof(PersonResponse.Adress),
                    nameof(PersonResponse.DateOfBirth)
                    };
                    if(searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.FirstName) ;
                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
