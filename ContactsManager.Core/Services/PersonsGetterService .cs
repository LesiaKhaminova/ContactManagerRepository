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
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
            
        public PersonsGetterService(IPersonsRepository personsRepository, 
            ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public virtual async Task<List<PersonResponse>> GetAllPersons()
            {
            /*
             List<PersonResponse> person_response_list = new List<PersonResponse>();

             person_response_list = _dbContext.Persons.ToList().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();

             return person_response_list;*/
            _logger.LogInformation("Get all persons of the personsService");

            var persons =await _personsRepository.GetAllPersons();

            return persons.Select(temp => temp.ToPersonResponse()).ToList();
            }

        public virtual async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
            {
                if (personId == null) return null;

                Person? person =await _personsRepository.GetPersonByPersonId(personId.Value);

                if (person == null) return null;

                return person.ToPersonResponse();
        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
            {
            _logger.LogInformation("GetFilteredPersons of the personsService");
            List<Person> persons = new List<Person>();
            using (Operation.Time("Time for Filtered Persons Database"))
            {
                persons = searchBy switch
                {
                    nameof(Person.FirstName) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                        temp.FirstName.Contains(searchString)),

                    nameof(Person.Email) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                    temp.Email.Contains(searchString)),

                    nameof(Person.LastName) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                        temp.LastName.Contains(searchString)),

                    nameof(Person.Adress) =>
                             await _personsRepository.GetFilteredPersons(temp =>
                             temp.Adress.Contains(searchString)),

                    nameof(Person.Gender) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                        temp.Gender.Contains(searchString)),

                    nameof(Person.DateOfBirth) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                        temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                    nameof(Person.CountryId) =>
                         await _personsRepository.GetFilteredPersons(temp =>
                         temp.Country.CountryName.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons()
                };
            }

            _diagnosticContext.Set("persons", persons);
                return persons.Select(temp => temp.ToPersonResponse()).ToList();
            }

        public virtual async Task<MemoryStream> GetPersonCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            CsvWriter csvWriter = new CsvWriter(streamWriter,csvConfiguration );

            csvWriter.WriteField(nameof(PersonResponse.FirstName));
            csvWriter.WriteField(nameof(PersonResponse.LastName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.CountryName));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.NextRecord();
            List<PersonResponse> persons_list = await GetAllPersons();

            foreach(PersonResponse person in persons_list)
            {
                csvWriter.WriteField(person.FirstName);
                csvWriter.WriteField(person.LastName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.CountryName);
                csvWriter.WriteField(person.Age);

                if(person.DateOfBirth != null)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else
                    csvWriter.WriteField("");
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            memoryStream.Position = 0;
            return memoryStream;

            /*
            csvWriter.WriteHeader<PersonResponse>();
            csvWriter.NextRecord();

            List<PersonResponse> persons_list = _dbContext.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();
            await csvWriter.WriteRecordsAsync(persons_list);

            memoryStream.Position= 0;
            return memoryStream; */
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet1");
                excelWorksheet.Cells["A1"].Value = "First Name";
                excelWorksheet.Cells["B1"].Value = "Last Name";
                excelWorksheet.Cells["C1"].Value = "Adress";
                excelWorksheet.Cells["D1"].Value = "Email";
                excelWorksheet.Cells["E1"].Value = "Country";
                excelWorksheet.Cells["F1"].Value = "Age";
                excelWorksheet.Cells["G1"].Value = "Date Of Birth";

                using(ExcelRange headerCells = excelWorksheet.Cells["A1:G1"])
                {
                    headerCells.Style.Fill.PatternType =
                        OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor
                        (System.Drawing.Color.DarkOrange);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> list_person = await GetAllPersons();
                foreach(PersonResponse person in list_person)
                {
                    excelWorksheet.Cells[$"A{row}"].Value = person.FirstName;
                    excelWorksheet.Cells[$"B{row}"].Value = person.LastName;
                    excelWorksheet.Cells[$"C{row}"].Value = person.Adress;
                    excelWorksheet.Cells[$"D{row}"].Value = person.Email;
                    excelWorksheet.Cells[$"E{row}"].Value = person.CountryName;
                    excelWorksheet.Cells[$"F{row}"].Value = person.Age;
                    if(person.DateOfBirth!= null) 
                        excelWorksheet.Cells[$"G{row}"].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    row++;
                }
                excelWorksheet.Cells[$"A1:F{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
                memoryStream.Position = 0;
                return memoryStream;
            };
        }
    }
}
