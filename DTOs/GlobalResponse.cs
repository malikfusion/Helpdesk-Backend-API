namespace Helpdesk_Backend_API.DTOs
{
    public class GlobalResponse<T>
    {
        //public int Status { get; set; }
        //public string StatusText { get; set; }
        public T Data { get; set; }
        public List<ErrorItemModel> Errors { get; set; }

        public GlobalResponse()
        {
            Errors = new List<ErrorItemModel>();
        }

        public static GlobalResponse<T> Fail(string errorMessage)
        {
            return new GlobalResponse<T>()
            {
                Errors = new List<ErrorItemModel>()
                {
                    new ErrorItemModel()
                    {
                        Key = "Failed",
                        ErrorMessages = new List<string>(){errorMessage}
                    }
                }
            };
        }

        public static GlobalResponse<T> Success(T data, string message)
        {
            return new GlobalResponse<T> { /*StatusText = message,*/ Data = data };
        }
    }
}
