using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ServiceContracts.Enums;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositiruContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsSorterService : IPersonsSorterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsSorterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
            
        public PersonsSorterService(IPersonsRepository personsRepository, 
            ILogger<PersonsSorterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> all_persons, string sortBy, SortOrderOptions sortOrder)
            {
                _logger.LogInformation("Get sorted persons of the personsService");
               if (string.IsNullOrEmpty(sortBy)) return all_persons;

                 List<PersonResponse> sorted_persons_list = (sortBy, sortOrder) switch
                {
                    (nameof(PersonResponse.FirstName), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.FirstName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.FirstName), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.FirstName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.LastName), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.LastName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.LastName), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.LastName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Adress), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.Adress, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Adress), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.Adress, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.DateOfBirth).ToList(),

                    (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.Age).ToList(),

                    (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.Age).ToList(),

                    (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                    all_persons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                    (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                    all_persons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                    _ => all_persons
                };
                return sorted_persons_list;
            }

    }
}
