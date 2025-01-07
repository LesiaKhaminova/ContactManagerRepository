using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person name can't be blank ")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Person last name can't be blank ")]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email can't be blank ")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public Guid? CountryId { get; set; }
        public string? Adress { get; set; }
        public GenderOptions? Gender { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person() { FirstName = this.FirstName, LastName = this.LastName , 
                DateOfBirth = this.DateOfBirth ,Email = this.Email, CountryId = this.CountryId, 
                Adress = this.Adress, Gender = this.Gender.ToString(),
                ReceiveNewsLetters = this.ReceiveNewsLetters 
            };
        }
    }
}
