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
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
            
        public PersonsAdderService(IPersonsRepository personsRepository, 
            ILogger<PersonsAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
                if (personAddRequest == null) throw new ArgumentNullException(nameof(personAddRequest));

                //ModelValidation
                ValidationHelper.ModelValidation(personAddRequest);

                Person new_person = personAddRequest.ToPerson();
                new_person.PersonId = Guid.NewGuid();

                await _personsRepository.AddPerson(new_person);

                return new_person.ToPersonResponse();
        }

    }
}
