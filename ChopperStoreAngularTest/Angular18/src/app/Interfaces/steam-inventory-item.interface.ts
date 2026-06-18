export interface SteamInventoryItem {
  assetId: string;
  name: string;
  photoUrl?: string | null;
  rarity?: string | null;
  rarityColor?: string | null;
  exterior?: string | null;
  skinFloat?: number | null;
  pattern?: number | null;
  floatDisponible: boolean;
  tradable: boolean;
}
