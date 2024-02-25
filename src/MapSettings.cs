
using System.Text.Json.Serialization;

public class MapSettings
{
    [JsonPropertyName("AutoDetection")] public bool AutoDetection { get; set; } = true;
    [JsonPropertyName("RespawnTime")] public float RespawnTime { get; set; } = 0.0f;
}