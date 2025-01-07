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
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsDeleterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
            
        public PersonsDeleterService(IPersonsRepository personsRepository, 
            ILogger<PersonsDeleterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }



        public async Task<bool> DeletePerson(Guid? personId)
        {
                if (personId == null) throw new ArgumentNullException(nameof(personId));

            Person? matching_person = await _personsRepository.GetPersonByPersonId(personId.Value);
                if (matching_person == null) return false;

                await _personsRepository.DeletePersonByPersonId(personId.Value);
                return true;
        }
    }
}
