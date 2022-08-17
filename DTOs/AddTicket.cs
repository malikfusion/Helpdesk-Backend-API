using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class AddTicket : BaseEntityDto
    {
        //[Required]
        //public string TicketId { get; set; }
        //public string StaffId { get; set; }

        [Required]
        public string OrganizationId { get; set; }
    }
}
