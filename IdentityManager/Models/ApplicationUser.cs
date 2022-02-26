using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityManager.Models
{
    
    /// <summary>
    /// Here, deriving from IdentityUser determines the behavier of this class !
    /// </summary>
    public class ApplicationUser:IdentityUser
    
    {
        [Required]
        public string Name { get; set; }
    }
}