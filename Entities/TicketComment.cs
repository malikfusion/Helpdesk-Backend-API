using Helpdesk_Backend_API.Entities.NonDbEntities;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.Entities
{
    public class TicketComment : DbEntity
    {
        [Required]
        public string TicketId { get; set; }
        public Ticket Ticket { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }
        public HelpDeskUser User { get; set; }
    }
}
