using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class AddProduct : BaseEntityDto
    {
        [Required]
        public string ProjectId { get; set; }


        [Required]
        public string OrganizationId { get; set; }
    }
}
