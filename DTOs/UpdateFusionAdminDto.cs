using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class UpdateFusionAdminDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }

}
