using System;
using System.Collections.Generic;
using Helpdesk_Backend_API.Utilities;

namespace Helpdesk_Backend_API.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string OrganizationId { get; set; }
    }
}
