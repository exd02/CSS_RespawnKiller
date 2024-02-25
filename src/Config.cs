using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace RespawnKiller
{
    public class RespawnKillerConfig : BasePluginConfig
    {
        [JsonPropertyName("ChatPrefix")] public string ChatPrefix { get; set; } = "[RespawnKill]";
        [JsonPropertyName("TimeDeadScreen")] public float TimeDeadScreen { get; set; } = 0.5f;
        [JsonPropertyName("TimeBtwPlayerDeathsToDetectRespawnKill")] public float TimeBtwPlayerDeathsToDetectRespawnKill { get; set; } = 5.5f;
        [JsonPropertyName("LetPluginDecideForRoundEndConditions")] public bool LetPluginDecideForRoundEndConditions { get; set; } = true;
        [JsonPropertyName("DebugMessages")] public bool DebugMessages { get; set; } = false;
    }
}