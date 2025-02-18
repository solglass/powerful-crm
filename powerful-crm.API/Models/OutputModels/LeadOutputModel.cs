﻿using System;
using System.Collections.Generic;

namespace powerful_crm.API.Models.OutputModels
{
    public class LeadOutputModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BirthDate { get; set; }
        public CityOutputModel City { get; set; }
        public List<AccountOutputModel> Accounts { get; set; }
    }
}
