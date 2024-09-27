namespace KioskService.Core.DTO
{

    public class Response
    {
        public int statusCode { get; set; }
        public string? message { get; set; }
        public string? errorType { get; set; }
        public string? stackTrace { get; set; }
        public string deviceId { get; set; } = default!;
        public DateTime? date { get; set; }
    }

    public class Response<T> : Response
    {
        public T? data { get; set; }
    }
}
