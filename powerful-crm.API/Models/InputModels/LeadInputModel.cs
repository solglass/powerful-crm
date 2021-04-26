using powerful_crm.API.Attributes;
using powerful_crm.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class LeadInputModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Login { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 6)]
        public string Email { get; set; }
        [Required]
        [StringLength(20)]
        public string Phone { get; set; }
        [Required]
        [CustomDateTimeValidation]
        public string BirthDate { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CityId { get; set; }
    }
}
