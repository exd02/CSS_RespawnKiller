using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace RespawnKiller;

public partial class RespawnKiller
{
	public void InitializeEventHandles()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath,HookMode.Post);
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
    }

    HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        
        
        double thisDeathTime = Server.EngineTime;

#if DEBUG
        Server.PrintToConsole($"[RespawnKiller] Player in the slot { player.Slot } dead, ThisDeathTime: { thisDeathTime }, LastDealthTime: { lastDeathTime[player.Slot] }!");
#endif

        if (thisDeathTime - lastDeathTime[player.Slot] < 5 && autoDetectRespawnKill)
        {
            Server.PrintToChatAll($"[RespawnKiller] Auto Respawn Kill Detection has been activated!");
            Server.ExecuteCommand($"mp_respawn_on_death_ct false");
            Server.ExecuteCommand($"mp_respawn_on_death_t false");
        }

        lastDeathTime[player.Slot] = thisDeathTime;

        return HookResult.Continue;
    }

    HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        
        if (!player.IsValid || !player.IsPlatform)
        {
            return HookResult.Continue;
        }

        Server.PrintToConsole($"[RespawnKiller] Player in the slot { player.Slot } respawned");

        return HookResult.Continue;
    }

    [GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
        CCSPlayerController? player = @event.Userid;

#if DEBUG
		Server.PrintToConsole($"[RespawnKiller] Player in the slot { player.Slot } has disconnected, cleaning lastDeathTime for the Slot.");
#endif
        lastDeathTime[player.Slot] = 0.0;

        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnEventRoundStartPre(EventRoundStart @event, GameEventInfo info)
    {

#if DEBUG
		Server.PrintToConsole($"[RespawnKiller] The round has started, changing respawn variables back to enabled.");
#endif

        Server.ExecuteCommand($"mp_respawn_on_death_ct true");
        Server.ExecuteCommand($"mp_respawn_on_death_t true");

        return HookResult.Continue;
    }
}