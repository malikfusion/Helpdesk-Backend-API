using Helpdesk_Backend_API.Entities.NonDbEntities;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.Entities
{
    public class FusionAdmin : DbEntity
    {
        [Required]
        public string UserId { get; set; }
        public HelpDeskUser User { get; set; }
    }
}
