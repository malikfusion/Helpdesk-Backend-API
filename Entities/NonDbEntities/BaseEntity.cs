using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.Entities.NonDbEntities
{
    public class BaseEntity : DbEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
