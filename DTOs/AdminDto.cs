using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class CreateFusionAdminDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }

}
