namespace KioskService.Persistance.Entities
{
    public class PaymentEntity
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = default!;
        public string Organization { get; set; } = default!;
        public double Sum { get; set; } = default!;
        public DateTime TimeStamp { get; set; }
        public string BankId { get; set; } = default!;
        public string PaymentWay { get; set; } = default!;
        public List<string> PaymentObjects { get; set; } = new List<string>();
        public string Check { get; set; } = default!;
        public bool IsValid { get; set; }
    }
}
