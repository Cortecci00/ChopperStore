namespace ChopperStoreAngularTest.Models
{
    public class Transaction
    {
        public int Id {  get; set; }
        public int UserId { get; set; }
        public User user { get; set; }
        public List<TransactionItem> items { get; set; } = new();
        public double totalPrice { get; set; }
        public DateTime transactionDate { get; set; }
        public string PaymentStatus { get; set; } = "pending";
        public string? MercadoPagoPreferenceId { get; set; }
        public string? MercadoPagoPaymentId { get; set; }
        public string DeliveryStatus { get; set; } = "waiting";

    }
}
