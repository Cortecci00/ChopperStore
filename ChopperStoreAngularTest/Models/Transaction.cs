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

    }
}
