using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk_Backend_API.Entities
{
    public class HelpDeskUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public string Password { get; set; }

        public string ProfileImageUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [Required]
        public string RoleName { get; set; }

        public string RefreshTokenKey { get; set; }
        public DateTime RefreshTokenExpirytime { get; set; }

        public bool IsActive { get; set; }
    }
}
