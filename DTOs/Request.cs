using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.DTOs
{
    public class ClaimsToRoleModel
    {
        public List<string> Claims { get; set; }
        public string RoleName { get; set; }
    }
}
