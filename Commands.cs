using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace RespawnKiller;

public partial class RespawnKiller
{
    [ConsoleCommand("css_setmaprespawntime", "Respawn time in this map")]
    [CommandHelper(minArgs: 1, usage: "[seconds]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/cvar")]
    public void OnSetMapRespawnTimeCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {

        int respawnTime;
        if(!int.TryParse(commandInfo.GetArg(1), out respawnTime))
        {
            Server.PrintToConsole($"[RespawnKiller] Incorrect Usage.");
            return;
        }

        Config.RespawnTime = respawnTime;

        if (player != null)
            player.PrintToChat($"[RespawnKiller] Respawn time in the current map ({ Server.MapName }) has been set to { Config.RespawnTime } seconds.");

        Server.PrintToConsole($"[RespawnKiller] Respawn time in the current map ({ Server.MapName }) has been set to { Config.RespawnTime } seconds.");
    }

    [ConsoleCommand("css_autodetectrespawnkill", "Enable/Disable Auto-Detection for respawn kill.")]
    [CommandHelper(minArgs: 1, usage: "[0/1]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/cvar")]
    public void OnSetMapAutoDetectCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        int autoDetectArg;
        if(!int.TryParse(commandInfo.GetArg(1), out autoDetectArg))
        {
            Server.PrintToConsole($"[RespawnKiller] Incorrect Usage.");
            return;
        }

        Config.AutoDetection = autoDetectArg != 0;
        if (player != null)
            player.PrintToChat($"[RespawnKiller] Respawn auto-detection for the map ({ Server.MapName }) has been set to { (Config.AutoDetection ? "true" : "false") }.");
        
        Server.PrintToConsole($"[RespawnKiller] Respawn auto-detection for the map ({ Server.MapName }) has been set to { (Config.AutoDetection ? "true" : "false") }.");
    }
}