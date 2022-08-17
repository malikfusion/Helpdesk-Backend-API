namespace Helpdesk_Backend_API.DTOs
{
    public class ServiceResponse<T>
    {
        public ResponseType ResponseType { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public enum ResponseType
    {
        Failed,
        Success,
        AlreadyCompleted,
        Duplicate,
        NotFound
    }
}
