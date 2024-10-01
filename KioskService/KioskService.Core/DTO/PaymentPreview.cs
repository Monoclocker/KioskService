namespace KioskService.Core.DTO
{
    public class PaymentPreview
    {
        public int id { get; set; }
        public DateTime? localDate { get; set; }
        public double sum { get; set; }
    }
}
