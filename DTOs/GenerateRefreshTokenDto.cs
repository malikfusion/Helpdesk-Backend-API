using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class GenerateRefreshTokenDto
    {
        [Required]
        public string CurrentJWT { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
