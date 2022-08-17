using Helpdesk_Backend_API.Entities.NonDbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }

        [Required]
        public string ProjectId { get; set; }
        public Project Project { get; set; }

    }
}