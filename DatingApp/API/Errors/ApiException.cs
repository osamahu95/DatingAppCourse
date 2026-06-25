namespace API.Errors
{
    public class ApiException
    {
        public int StatusCode { get; private set; }
        public string Message { get; private set; }
        public string? Details { get; private set; }

        private ApiException () {}

        public static ApiException Create(int StatusCode, string Message, string? Details = null)
        {
            return new ApiException
            {
                StatusCode = StatusCode,
                Message = Message,
                Details = Details
            };
        }
    }
}
