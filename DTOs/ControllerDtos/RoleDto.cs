using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs.ControllerDtos
{
    public class CreateRoleDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}
