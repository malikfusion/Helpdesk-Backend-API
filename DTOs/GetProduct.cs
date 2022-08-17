namespace Helpdesk_Backend_API.DTOs
{
    public class GetProduct
    {
        public string Id { get; set; }
        public GetProject Project { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
