using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;

using CounterStrikeSharp.API.Modules.Memory;


namespace RespawnKiller;

public partial class RespawnKiller
{
	public void InitializeEventHandles()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath,HookMode.Post);
    }

    HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (!canRespawn) return HookResult.Continue;

        CCSPlayerController? player = @event.Userid;

        if (player == null) return HookResult.Continue;
        
        double thisDeathTime = Server.EngineTime;
        double timeAlive = thisDeathTime - lastDeathTime[player.Slot];
        lastDeathTime[player.Slot] = thisDeathTime;

        if (timeAlive < 5 && Config.AutoDetection)
        {
            Server.PrintToChatAll($"[RespawnKiller] Auto Respawn Kill Detection has been activated!");
            canRespawn = false;
        }
        
        if (canRespawn)
        {
            Server.PrintToConsole($"[RespawnKiller] Respawn active, spawning player \"{player.PlayerName}\" in {Config.TimeDeadScreen} seconds!");
            AddTimer(Config.TimeDeadScreen, () => { Respawn(player); }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
        CCSPlayerController? player = @event.Userid;

        if (!player.IsValid)
        {
            return HookResult.Continue;
        }

#if DEBUG
		Server.PrintToConsole($"[RespawnKiller] Player in the slot { player.Slot } has disconnected, cleaning lastDeathTime for the Slot.");
#endif
        lastDeathTime[player.Slot] = 0.0;

        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnEventRoundStartPre(EventRoundStart @event, GameEventInfo info)
    {
        // Change the respawn variables back to normal
        Server.PrintToConsole($"[RespawnKiller] Round Starting, turning on respawn...");
        canRespawn = true;

        // Create a timer to set the respawn variable to false
        if (Config.RespawnTime > 0)
        {
            // 'Timer' is an ambiguous reference between 'CounterStrikeSharp.API.Modules.Timers.Timer' and 'System.Threading.Timer'
            CounterStrikeSharp.API.Modules.Timers.Timer timerToDisableRespawn = AddTimer(Config.RespawnTime, () => {
                Server.PrintToConsole($"[RespawnKiller] {Config.RespawnTime} seconds has passed since round Start. Turning Off Respawn...");
                canRespawn = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

    public void Respawn(CCSPlayerController player)
    {
        VirtualFunction.CreateVoid<CCSPlayerController>(player.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(player);
    }
}