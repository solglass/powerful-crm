using powerful_crm.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Models
{
    public class SearchLeadDto
    {
        public string FirstName { get; set; }
        public SearchType TypeSearchFirstName { get; set; }
        public string LastName { get; set; }
        public SearchType TypeSearchLastName { get; set; }
        public string Email { get; set; }
        public SearchType TypeSearchEmail { get; set; }
        public string Login { get; set; }
        public SearchType TypeSearchLogin { get; set; }
        public string Phone { get; set; }
        public SearchType TypeSearchPhone { get; set; }
        public DateTime? StartBirthDate { get; set; }
        public DateTime? EndBirthDate { get; set; }
        public CityDto City{ get; set; }
        public SearchType TypeSearchCityName { get; set; }
    }
}
