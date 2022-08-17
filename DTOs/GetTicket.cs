namespace Helpdesk_Backend_API.DTOs
{
    public class GetTicket
    {
        public string Id { get; set; }
        public GetStaff Staff { get; set; }
        public GetOrganization Organization { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
