using KioskService.Core.Enums;

namespace KioskService.Persistance.Entities
{
    public class ResultsEntity
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = default!;
        public double Sum { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string? Check { get; set; }
    }
}
