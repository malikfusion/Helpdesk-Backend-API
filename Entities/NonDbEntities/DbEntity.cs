using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities.NonDbEntities
{
    public class DbEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
        public DateTime? DateModified { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
