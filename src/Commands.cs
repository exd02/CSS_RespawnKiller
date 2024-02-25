using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API;

namespace RespawnKiller;

public partial class RespawnKiller
{
    [ConsoleCommand("css_setmaprespawntime", "Respawn time in this map")]
    [CommandHelper(minArgs: 1, usage: "<seconds>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/cvar")]
    public void OnSetMapRespawnTimeCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        int respawnTimeArg;
        if(!int.TryParse(commandInfo.GetArg(1), out respawnTimeArg))
        {
            PrintColored($"Incorrect Usage.", player);
            return;
        }

        respawnTime = respawnTimeArg;
        
        PrintColored($"Respawn Time for the map ({ Server.MapName }) has been set to { respawnTime } seconds..", player);
        SaveMapConfig(autoDetectRespawnKill, respawnTime);
    }

    [ConsoleCommand("css_autodetectrespawnkill", "Enable/Disable Auto-Detection for respawn kill.")]
    [CommandHelper(minArgs: 1, usage: "<0/1>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/cvar")]
    public void OnSetMapAutoDetectCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        int autoDetectArg;
        if(!int.TryParse(commandInfo.GetArg(1), out autoDetectArg))
        {
            PrintColored($"Incorrect Usage.", player);
            return;
        }

        autoDetectRespawnKill = autoDetectArg != 0;

        PrintColored($"Auto-detection to respawn-kill for the map ({ Server.MapName }) has been set to { autoDetectRespawnKill }.", player);
        SaveMapConfig(autoDetectRespawnKill, respawnTime);
    }
}