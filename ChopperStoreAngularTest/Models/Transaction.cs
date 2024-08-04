namespace ChopperStoreAngularTest.Models
{
    public class Transaction
    {
        public int Id {  get; set; }
        public User user { get; set; }
        public Item items { get; set; }
        public double totalPrice { get; set; }
        public DateTime transactionDate { get; set; }

    }
}
