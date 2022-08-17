using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities.NonDbEntities
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        public string Token { get; set; }
        public string JWTId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateExpires { get; set; }
        public bool IsRevoked { get; set; }
        public string UserId { get; set; }
        public HelpDeskUser User { get; set; }
    }
}
