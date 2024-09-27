namespace KioskService.Core.Models
{
    public class Payment
    {
        public Guid id { get; set; }
        public string status { get; set; } = default!;
        public string organization { get; set; } = default!;
        public double sum { get; set; } = default!;
        public DateTime localTime { get; set; }
        public DateTime utcTime { get; set; }
        public string idemptencyENR { get; set; } = default!;
        public string paymentWay { get; set; } = default!;
        public List<string> paymentObjects { get; set; } = new List<string>();
        public string check { get; set; } = default!;
    }
}
