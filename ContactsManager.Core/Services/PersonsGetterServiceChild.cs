using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositiruContracts;
using Serilog;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsGetterServiceChild : PersonsGetterService
    {
        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger,
            IDiagnosticContext diagnosticContext) : base(personsRepository, logger, diagnosticContext) { }
        public async override Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet1");
                excelWorksheet.Cells["A1"].Value = "First Name";
                excelWorksheet.Cells["B1"].Value = "Last Name";
                excelWorksheet.Cells["C1"].Value = "Age";
                excelWorksheet.Cells["D1"].Value = "Date Of Birth";

                using (ExcelRange headerCells = excelWorksheet.Cells["A1:G1"])
                {
                    headerCells.Style.Fill.PatternType =
                        OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor
                        (System.Drawing.Color.DarkOrange);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> list_person = await GetAllPersons();
                foreach (PersonResponse person in list_person)
                {
                    excelWorksheet.Cells[$"A{row}"].Value = person.FirstName;
                    excelWorksheet.Cells[$"B{row}"].Value = person.LastName;
                    excelWorksheet.Cells[$"C{row}"].Value = person.Age;
                    if (person.DateOfBirth != null)
                        excelWorksheet.Cells[$"D{row}"].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    row++;
                }
                excelWorksheet.Cells[$"A1:F{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
                memoryStream.Position = 0;
                return memoryStream;

            }
        }
    }
}
