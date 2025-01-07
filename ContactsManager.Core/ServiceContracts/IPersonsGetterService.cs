using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;

namespace ServiceContracts
{   /// <summary>
/// represents buisness logic for manipulating person entity
/// </summary>
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Returns all existing persons 
        /// </summary>
        /// <returns> returns a list of objects of personResponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns persons obj based on the given id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Returns the matching person obj as personResponse</returns>
        Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons on the given search as PersonResponse list</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns person as CSV
        /// </summary>
        /// <returns></returns>
        Task<MemoryStream> GetPersonCSV();

        /// <summary>
        /// Returns person as Excel
        /// </summary>
        /// <returns>Returns the memory with Excel data of persons</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
