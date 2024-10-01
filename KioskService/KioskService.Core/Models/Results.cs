namespace KioskService.Core.Models
{
    public class Results
    {
        public double sum { get; set; }
        public string deviceId { get; set; } = default!;
        public DateTime localTime { get; set; }
        public DateTime utcTime { get; set; }
        public string? check { get; set; }  
    }
}
