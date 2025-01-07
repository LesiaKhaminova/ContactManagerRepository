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
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Addds new person to the list of persons
        /// </summary>
        /// <param name="personAddRequest"></param>
        /// <returns>returns the same persons details, along with newly generated person id</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    }
}
