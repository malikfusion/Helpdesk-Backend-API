using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.DTOs
{
    public class LoginRequestModel
    {
        [Display(Name ="Username/Email")]
        public string UserName { get; set; }

        [Display(Name ="Password")]
        public string Password { get; set; }
        
    }
}
