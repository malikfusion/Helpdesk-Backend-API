using System.Collections.Generic;

namespace Helpdesk_Backend_API.DTOs
{
    public class ErrorItemModel
    {
        public string Key { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
