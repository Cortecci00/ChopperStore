namespace ChopperStoreAngularTest.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User user { get; set; }
        public List<Item> items { get; set; } = new();
    }
}
