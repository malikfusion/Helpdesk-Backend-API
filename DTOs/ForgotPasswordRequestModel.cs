using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class ForgotPasswordRequestModel
    {
        [Required]
        public string Email { get; set; }
        public object Origin { get; internal set; }
    }
}
