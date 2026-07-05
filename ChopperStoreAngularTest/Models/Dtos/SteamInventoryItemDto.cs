namespace ChopperStoreAngularTest.Models.Dtos
{
    public class SteamInventoryItemDto
    {
        public string AssetId { get; set; }
        public string Name { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Rarity { get; set; }
        public string? RarityColor { get; set; }
        public string? Exterior { get; set; }
        public float? SkinFloat { get; set; }
        public int? Pattern { get; set; }
        public bool FloatDisponible { get; set; }
        public bool Tradable { get; set; }
        public string? InspectLink { get; set; }
    }
}
