using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API;

namespace RespawnKiller;

public partial class RespawnKiller
{
    [ConsoleCommand("css_setmaprespawntime", "Respawn time in this map")]
    [RequiresPermissions("@css/cvar")]
    public void OnSetMapRespawnTimeCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (commandInfo.ArgCount == 1)
		{
            PrintColored(Localizer["rk.current.respawn.time.value", Server.MapName, respawnTime], player);
            return;
        }

        int respawnTimeArg;
        if(!int.TryParse(commandInfo.GetArg(1), out respawnTimeArg))
        {
            PrintColored(Localizer["rk.incorrect.arg"], player);
            return;
        }

        respawnTime = respawnTimeArg;

        PrintColored(Localizer["rk.changed.respawn.time", Server.MapName, respawnTimeArg], player);

        SaveMapConfig(autoDetectRespawnKill, respawnTime);
    }

    [ConsoleCommand("css_autodetectrespawnkill", "Enable/Disable Auto-Detection for respawn kill.")]
    [RequiresPermissions("@css/cvar")]
    public void OnSetMapAutoDetectCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (commandInfo.ArgCount == 1)
		{
            PrintColored(Localizer["rk.current.auto.detection.value", Server.MapName, autoDetectRespawnKill], player);
            return;
        }

        int autoDetectArg;
        if(!int.TryParse(commandInfo.GetArg(1), out autoDetectArg))
        {
            PrintColored(Localizer["rk.incorrect.arg"], player);
            return;
        }

        autoDetectRespawnKill = autoDetectArg != 0;

        PrintColored(Localizer["rk.changed.auto.detection", Server.MapName, autoDetectRespawnKill], player);

        SaveMapConfig(autoDetectRespawnKill, respawnTime);
    }
}