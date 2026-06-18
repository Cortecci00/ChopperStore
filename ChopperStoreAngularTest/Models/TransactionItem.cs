using System.Text.Json.Serialization;

namespace ChopperStoreAngularTest.Models
{
    public class TransactionItem
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        [JsonIgnore]
        public Transaction transaction { get; set; }
        public int? SkinId { get; set; }
        public Skin? skin { get; set; }
        public string SkinName { get; set; }
        public string? SkinPhotoUrl { get; set; }
        public int quantity { get; set; }
        public double unitPriceAtPurchase { get; set; }
    }
}
