using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace RespawnKiller
{
    public class RespawnKillerConfig : BasePluginConfig
    {
        [JsonPropertyName("AutoDetection")] public bool AutoDetection { get; set; } = true;
        [JsonPropertyName("RespawnTime")] public float RespawnTime { get; set; } = 0.0f;
        [JsonPropertyName("TimeDeadScreen")] public float TimeDeadScreen { get; set; } = 0.5f;
    }
}