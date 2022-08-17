namespace Helpdesk_Backend_API.DTOs
{
    public class GetProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GetOrganization Organization { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
