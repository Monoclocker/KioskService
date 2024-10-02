using KioskService.Core.Enums;

namespace KioskService.Core.DTO
{
    public class Request
    {
        public string deviceId { get; set; } = default!;
        public KioskTypes? kioskType { get; set; }
    }

    public class Request<T>: Request
    {
        public T data { get; set; } = default!;
    }
}
