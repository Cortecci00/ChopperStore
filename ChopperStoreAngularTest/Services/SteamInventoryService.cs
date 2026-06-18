using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ChopperStoreAngularTest.Models.Dtos;
using Microsoft.AspNetCore.WebUtilities;

namespace ChopperStoreAngularTest.Services
{
    public interface ISteamInventoryService
    {
        Task<List<SteamInventoryItemDto>> GetInventoryAsync(string tradeUrl);
    }

    public class SteamInventoryService : ISteamInventoryService
    {
        private const ulong SteamId64Base = 76561197960265728UL;

        private readonly HttpClient _httpClient;

        public SteamInventoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Steam");
        }

        public static ulong? ParseTradeUrlToSteamId64(string tradeUrl)
        {
            if (string.IsNullOrWhiteSpace(tradeUrl))
            {
                return null;
            }

            try
            {
                var uri = new Uri(tradeUrl);
                var query = QueryHelpers.ParseQuery(uri.Query);
                if (!query.TryGetValue("partner", out var partnerValues) || !uint.TryParse(partnerValues.ToString(), out var partner))
                {
                    return null;
                }

                return partner + SteamId64Base;
            }
            catch (Exception ex) when (ex is UriFormatException or ArgumentException)
            {
                return null;
            }
        }

        public async Task<List<SteamInventoryItemDto>> GetInventoryAsync(string tradeUrl)
        {
            var steamId64 = ParseTradeUrlToSteamId64(tradeUrl);
            if (steamId64 == null)
            {
                throw new InvalidOperationException("La Trade URL configurada no es válida");
            }

            var url = $"https://steamcommunity.com/inventory/{steamId64}/730/2?l=english&count=2000";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("No se pudo leer el inventario de Steam. Verificá que el inventario sea público.");
            }

            var inventory = await response.Content.ReadFromJsonAsync<SteamInventoryResponse>();
            if (inventory?.Assets == null || inventory.Descriptions == null)
            {
                return new List<SteamInventoryItemDto>();
            }

            var descriptionsByKey = inventory.Descriptions
                .GroupBy(d => (d.ClassId, d.InstanceId))
                .ToDictionary(g => g.Key, g => g.First());

            var propertiesByAssetId = (inventory.AssetProperties ?? new List<SteamAssetProperties>())
                .Where(p => p.AssetId != null)
                .ToDictionary(p => p.AssetId, p => p.Properties);

            var items = new List<SteamInventoryItemDto>();

            foreach (var asset in inventory.Assets)
            {
                if (!descriptionsByKey.TryGetValue((asset.ClassId, asset.InstanceId), out var description))
                {
                    continue;
                }

                var rarityTag = description.Tags?.FirstOrDefault(t => t.Category == "Rarity");
                var rarity = rarityTag?.LocalizedTagName;
                var rarityColor = rarityTag?.Color;
                var exterior = description.Tags?.FirstOrDefault(t => t.Category == "Exterior")?.LocalizedTagName;
                var photoUrl = description.IconUrl != null
                    ? $"https://community.cloudflare.steamstatic.com/economy/image/{description.IconUrl}"
                    : null;

                float? skinFloat = null;
                int? pattern = null;

                if (propertiesByAssetId.TryGetValue(asset.AssetId, out var properties) && properties != null)
                {
                    var floatValue = properties.FirstOrDefault(p => p.PropertyId == 2)?.FloatValue;
                    var patternValue = properties.FirstOrDefault(p => p.PropertyId == 1)?.IntValue;

                    if (floatValue != null && float.TryParse(floatValue, CultureInfo.InvariantCulture, out var parsedFloat))
                    {
                        skinFloat = parsedFloat;
                    }

                    if (patternValue != null && int.TryParse(patternValue, out var parsedPattern))
                    {
                        pattern = parsedPattern;
                    }
                }

                var name = description.MarketHashName ?? description.Name;
                if (exterior != null && name.EndsWith($"({exterior})"))
                {
                    name = name[..^$"({exterior})".Length].TrimEnd();
                }

                items.Add(new SteamInventoryItemDto
                {
                    AssetId = asset.AssetId,
                    Name = name,
                    PhotoUrl = photoUrl,
                    Rarity = rarity,
                    RarityColor = rarityColor,
                    Exterior = exterior,
                    SkinFloat = skinFloat,
                    Pattern = pattern,
                    FloatDisponible = skinFloat != null,
                    Tradable = description.Tradable == 1
                });
            }

            return items.OrderBy(i => i.Name).ToList();
        }

        private class SteamInventoryResponse
        {
            [JsonPropertyName("assets")]
            public List<SteamAsset>? Assets { get; set; }

            [JsonPropertyName("descriptions")]
            public List<SteamDescription>? Descriptions { get; set; }

            [JsonPropertyName("asset_properties")]
            public List<SteamAssetProperties>? AssetProperties { get; set; }
        }

        private class SteamAsset
        {
            [JsonPropertyName("assetid")]
            public string AssetId { get; set; } = "";

            [JsonPropertyName("classid")]
            public string ClassId { get; set; } = "";

            [JsonPropertyName("instanceid")]
            public string InstanceId { get; set; } = "";
        }

        private class SteamDescription
        {
            [JsonPropertyName("classid")]
            public string ClassId { get; set; } = "";

            [JsonPropertyName("instanceid")]
            public string InstanceId { get; set; } = "";

            [JsonPropertyName("icon_url")]
            public string? IconUrl { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("market_hash_name")]
            public string? MarketHashName { get; set; }

            [JsonPropertyName("tradable")]
            public int Tradable { get; set; }

            [JsonPropertyName("tags")]
            public List<SteamTag>? Tags { get; set; }
        }

        private class SteamTag
        {
            [JsonPropertyName("category")]
            public string Category { get; set; } = "";

            [JsonPropertyName("localized_tag_name")]
            public string LocalizedTagName { get; set; } = "";

            [JsonPropertyName("color")]
            public string? Color { get; set; }
        }

        private class SteamAssetProperties
        {
            [JsonPropertyName("assetid")]
            public string AssetId { get; set; } = "";

            [JsonPropertyName("asset_properties")]
            public List<SteamAssetProperty>? Properties { get; set; }
        }

        private class SteamAssetProperty
        {
            [JsonPropertyName("propertyid")]
            public int PropertyId { get; set; }

            [JsonPropertyName("int_value")]
            public string? IntValue { get; set; }

            [JsonPropertyName("float_value")]
            public string? FloatValue { get; set; }
        }
    }
}
