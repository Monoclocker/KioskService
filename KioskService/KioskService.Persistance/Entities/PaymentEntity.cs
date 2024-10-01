namespace KioskService.Persistance.Entities
{
    public class PaymentEntity
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Organization { get; set; } = default!;
        public double Sum { get; set; } = default!;
        public DateTime TimeStamp { get; set; }
        public string BankId { get; set; } = default!;
        public string PaymentWay { get; set; } = default!;
        public List<string> PaymentObjects { get; set; } = new List<string>();
        public bool IsValid { get; set; } = true;
        public string? Check { get; set; }
    }
}
