using Helpdesk_Backend_API.Entities.NonDbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities
{
    public class Organization : BaseEntity
    {
        public List<Project> Projects { get; set; }

        public Organization()
        {
            Projects = new List<Project>();
        }
    }
}