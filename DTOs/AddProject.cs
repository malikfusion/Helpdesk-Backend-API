using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class AddProject : BaseEntityDto
    {
        [Required]
        public string OrganizationId { get; set; }
    }
}
