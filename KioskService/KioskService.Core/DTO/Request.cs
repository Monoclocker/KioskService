using KioskService.Core.Enums;

namespace KioskService.Core.DTO
{
    public class Request
    {
        public required string deviceId { get; set; }
        public required KioskTypes kioskType { get; set; }
        public object? data { get; set; }
    }
}
