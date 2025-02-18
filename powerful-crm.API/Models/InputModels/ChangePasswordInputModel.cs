﻿using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class ChangePasswordInputModel
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}
