namespace Helpdesk_Backend_API.DTOs
{
    public class GetStaff
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string OrganizationId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
