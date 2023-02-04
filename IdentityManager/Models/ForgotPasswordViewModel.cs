using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models
{
    public class ForgotPasswordViewModel
    {
        //[Required]
        //[DataType(DataType.Password)]  // make input characters non-readable
        //[StringLength(8, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
