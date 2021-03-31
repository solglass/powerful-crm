using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class ChangePasswordInputModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
