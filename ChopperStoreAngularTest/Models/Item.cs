namespace ChopperStoreAngularTest.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int SkinId { get; set; }
        public Skin skin { get; set; }
        public int quantity { get; set; }
        public int ShoppingCartId { get; set; }

    }
}
