using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace RespawnKiller;

public partial class RespawnKiller : BasePlugin, IPluginConfig<RespawnKillerConfig>
{
    public override string ModuleName => "MG Respawn Killer";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "exd0001";
    public override string ModuleDescription => "Let you set custom timers for respawning in each map or set it to auto-detect.";

    public RespawnKillerConfig Config { get; set; } = new();
    public static bool bExecMapCfg = false;
    public static CounterStrikeSharp.API.Modules.Timers.Timer? timerToDisableRespawn;

    public static bool canRespawn = false;

    public static bool autoDetectRespawnKill = true;
    public static float respawnTime = 0.0f;

    public static string gameDir = "";

    // Server.MaxPlayers is causing crash!
    public static double[] lastDeathTime = new double[64];

    public void OnConfigParsed(RespawnKillerConfig config)
	{
        Config = config;
    }
    
    public override void Load(bool hotReload)
    {
        PrintConDebug($"Loading");
        InitializeEvents();

        gameDir = Server.GameDirectory;

        ValidateMapSettingsFolder();
    }
}