using powerful_crm.Core.Enums;
using System;

namespace powerful_crm.Core.Models
{
    public class LeadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public CityDto City { get; set; }
        public bool IsDeleted { get; set; }
        public Role Role { get; set; }

    }
}
