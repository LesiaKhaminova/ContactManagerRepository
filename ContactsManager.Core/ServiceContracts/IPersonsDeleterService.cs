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
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Returns true if deletion is successful otherwise return false
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<bool> DeletePerson(Guid? personId);
    }
}
