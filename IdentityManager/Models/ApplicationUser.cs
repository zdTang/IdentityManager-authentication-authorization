using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}