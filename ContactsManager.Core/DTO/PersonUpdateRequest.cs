using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents the DTO class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        public Guid? PersonId { get; set; }

        [Required(ErrorMessage = "Person name can't be blank ")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Person last name can't be blank ")]
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Email can't be blank ")]
        public string? Email { get; set; }
        public Guid? CountryId { get; set; }
        public string? Adress { get; set; }
        public GenderOptions? Gender { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person()
            {
                PersonId = this.PersonId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Email = this.Email,
                CountryId = this.CountryId,
                Adress = this.Adress,
                Gender = this.Gender.ToString(),
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }
}
