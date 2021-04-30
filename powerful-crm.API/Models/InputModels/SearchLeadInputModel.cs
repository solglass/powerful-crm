using powerful_crm.API.Attributes;
using powerful_crm.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class SearchLeadInputModel
    {
        [StringLength(100, MinimumLength = 0)]
        public string FirstName { get; set; }
        public SearchType TypeSearchFirstName { get; set; }
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        public SearchType TypeSearchLastName { get; set; }
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        public SearchType TypeSearchEmail { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string Login { get; set; }
        public SearchType TypeSearchLogin { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        public SearchType TypeSearchPhone { get; set; }
        [CustomStartAndEndDateTimeValidation]
        public string StartBirthDate { get; set; }
        [CustomStartAndEndDateTimeValidation]
        public string EndBirthDate { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string CityName { get; set; }
        public SearchType TypeSearchCityName { get; set; }
    }
}
