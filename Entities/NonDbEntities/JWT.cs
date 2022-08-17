using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Entities.NonDbEntities
{
    public class JWT
    {
        public string SigningKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public string TokenValidityInMinutes { get; set; }
        public string RefreshTokenValidityInDays { get; set; }

        public IEnumerable<string> ValidIssuers { get; set; }
        public IEnumerable<string> ValidAudiences { get; set; }
    }
}
