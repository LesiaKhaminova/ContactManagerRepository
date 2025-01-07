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
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
            
        public PersonsUpdaterService(IPersonsRepository personsRepository, 
            ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
                if (personUpdateRequest == null) throw new ArgumentNullException(nameof(personUpdateRequest));

                ValidationHelper.ModelValidation(personUpdateRequest);

                Person? matching_person = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId.Value);

                if (matching_person == null) throw new InvalidPersonIdException("Given person does not exist");

                matching_person.FirstName = personUpdateRequest.FirstName;
                matching_person.LastName = personUpdateRequest.LastName;
                matching_person.Adress = personUpdateRequest.Adress;
                matching_person.Email = personUpdateRequest.Email;
                matching_person.Gender = personUpdateRequest.Gender.ToString();
                matching_person.CountryId = personUpdateRequest.CountryId;
                matching_person.DateOfBirth = personUpdateRequest.DateOfBirth;
                matching_person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            await _personsRepository.UpdatePerson(matching_person);

            return matching_person.ToPersonResponse();
        }
    }
}
