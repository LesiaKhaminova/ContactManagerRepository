using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositiruContracts
{
    /// <summary>
    /// represents data access logic for managing Person entity
    /// </summary>
    public interface  IPersonsRepository
    {
        /// <summary>
        /// Adds a new person object to the data store
        /// </summary>
        /// <param name="person">person object to add</param>
        /// <returns>returns a person object after adding it to the data store</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>All persons in the table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns person object based on the person id ; otherwise returns null
        /// </summary>
        /// <param name="personId">person id to search</param>
        /// <returns>Matching person or null</returns>
        Task<Person?> GetPersonByPersonId(Guid personId);

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to chek</param>
        /// <returns>all matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person,bool>> predicate);

        /// <summary>
        /// Deletes a person based on the person id
        /// </summary>
        /// <param name="personId">Person id to search</param>
        /// <returns>Returns true, if the deletion is successful otherwise false</returns>
        Task<bool> DeletePersonByPersonId(Guid personId);

        /// <summary>
        /// Updates a person object (person name and other details ) based on the given person id
        /// </summary>
        /// <param name="person">person object to update</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
