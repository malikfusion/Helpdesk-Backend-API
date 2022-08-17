using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class BaseEntityDto
    {
        [Required]
        public string Name { get; set; }


        [Required]
        public string Description { get; set; }
    }
}
