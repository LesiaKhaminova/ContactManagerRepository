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
    public interface IPersonsUpdaterService
    {

        /// <summary>
        /// Updates the specifaied person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">person details to update, including person id</param>
        /// <returns>Returns the person obj after updation </returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
