﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models
{
    /// <summary>
    /// Be aware these Annotation should work with ASP Helper, or they are not functional 
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name="Password")]
        [DataType(DataType.Password)]
        [StringLength(120, ErrorMessage ="The {0} must be at least {2} characters long.",MinimumLength =6)]
        public string Password { get; set; }


        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string Name { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }
        public string RoleSelected { get; set; }
    }
}
