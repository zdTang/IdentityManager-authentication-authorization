﻿using System.ComponentModel.DataAnnotations;

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
        [StringLength(100, ErrorMessage ="The {0} must be at least {1} characters long.",MinimumLength =6)]
        public string Password { get; set; }


        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}