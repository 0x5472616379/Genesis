using System.Text.Json.Serialization;

namespace Genesis.Definitions;

public class ItemData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonPropertyName("bonuses")]
    public Bonuses? Bonuses { get; set; }
}