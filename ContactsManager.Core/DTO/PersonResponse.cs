using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Person Service
    /// </summary>
    public class PersonResponse
    {
        public Guid? PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Adress { get; set; }
        public string? Gender { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person_response = (PersonResponse)obj;
            return this.PersonId == person_response.PersonId && this.ReceiveNewsLetters == person_response.ReceiveNewsLetters &&
                this.Email == person_response.Email && this.Adress == person_response.Adress && this.CountryId == person_response.CountryId &&
                this.Age == person_response.Age && this.DateOfBirth == person_response.DateOfBirth && this.FirstName == person_response.FirstName &&
                this.LastName == person_response.LastName && this.Gender == person_response.Gender && this.CountryName == person_response.CountryName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $" PersonId: {PersonId}, \n FirstName: {FirstName}, \n" +
                $" LastName: {LastName}, \n DateOfBirth: {DateOfBirth}, \n" +
                $" Email: {Email}, \n CountryId: {CountryId}, \n" +
                $" CountryName: {CountryName}, \n Adress: {Adress}, \n Age: {Age}, " +
                $" Gender: {Gender}, \n ReceiveNewsLetters: {ReceiveNewsLetters}, \n";     
        }

        public PersonUpdateRequest ToPearsonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = this.PersonId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Email = this.Email,
                CountryId = this.CountryId,
                Adress = this.Adress,
                Gender =(GenderOptions) Enum.Parse(typeof(GenderOptions), this.Gender, true),
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }

    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse() {PersonId = person.PersonId, LastName = person.LastName,
            FirstName = person.FirstName, Adress= person.Adress, CountryId = person.CountryId,
            DateOfBirth= person.DateOfBirth, Email= person.Email,Gender= person.Gender,
            ReceiveNewsLetters= person.ReceiveNewsLetters, 
            Age = person.DateOfBirth != null ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
            CountryName = person.Country?.CountryName};
        }

    }
    
    
}
