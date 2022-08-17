using Helpdesk_Backend_API.Entities.NonDbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities
{
    public class Staff : DbEntity
    {
        [Required]
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public string UserId { get; set; }
        public HelpDeskUser User { get; set; }
    }
}