﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid? PersonId { get; set; }

        [StringLength(40)]
        public string? FirstName { get; set; }

        [StringLength(40)]
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }
        public Guid? CountryId { get; set; }

        [StringLength(100)]
        public string? Adress { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public string? TIN { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Name: {FirstName}, Surname: {LastName}, DateOfBirth: {DateOfBirth?.ToString("MM/dd/yyyy")}," +
                $"Adress: {Adress}, Gender: {Gender}, Country: {Country?.CountryName}, Email: {Email}, Receive News Letters: {ReceiveNewsLetters} ";
        }
    }
}
