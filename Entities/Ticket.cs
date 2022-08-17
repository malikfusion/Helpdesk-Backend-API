using Helpdesk_Backend_API.Entities.Enums;
using Helpdesk_Backend_API.Entities.NonDbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public string ProductId {get; set; }
        public Product Product { get; set; }

        public string ProjectId {get;set; }
        public Project Project { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Pending;

        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        public string StaffAssignedToId { get; set; }

        public Staff StaffAssignedTo { get; set; }

        //[Required]
        //public string StaffId { get; set; }
        //public Staff Staff { get; set; }

        //public string TicketId { get; set; }
        //public Ticket TicketName { get; set; }
        
        public bool IsCompleted { get; set; } = false;

        public DateTime? DateAssigned { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        public List<TicketComment> Comments { get; set; }

    }



}