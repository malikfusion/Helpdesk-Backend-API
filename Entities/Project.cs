using Helpdesk_Backend_API.Entities.NonDbEntities;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.Entities
{
    public class Project : BaseEntity
    {
        [Required]
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public List<Product> Products { get; set; }
    }
}
