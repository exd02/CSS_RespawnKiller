using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;

namespace RespawnKiller;

public partial class RespawnKiller
{
	public void InitializeEvents()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath,HookMode.Post);

        RegisterListener<Listeners.OnMapStart>(OnMapStart);
    }

    private void OnMapStart(string mapName)
    {
        Server.NextFrame(() =>
        {
            // TODO: exec commands for the map name here, add delay 1s to execute

            // reset the last death time var
            for (int i = 0; i < lastDeathTime.Length; i++)
            {
                lastDeathTime[i] = 0.0;
            }
        });
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
            Server.PrintToChatAll($"{ Config.ChatPrefix } Auto Respawn Kill Detection has been activated!");
            canRespawn = false;
        }

        if (canRespawn)
        {
            Server.PrintToConsole($"{ Config.ChatPrefix } Respawn active, spawning player \"{ player.PlayerName }\" in {Config.TimeDeadScreen} seconds!");
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
		Server.PrintToConsole($"{ Config.ChatPrefix } Player in the slot { player.Slot } has disconnected, cleaning lastDeathTime for the Slot.");
#endif
        lastDeathTime[player.Slot] = 0.0;

        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnEventRoundStartPre(EventRoundStart @event, GameEventInfo info)
    {
        // Change the respawn variable back to on
        canRespawn = true;

        // Create a timer to set the respawn variable to false
        if (Config.RespawnTime > 0.0)
        {
            // 'Timer' is an ambiguous reference between 'CounterStrikeSharp.API.Modules.Timers.Timer' and 'System.Threading.Timer'
            CounterStrikeSharp.API.Modules.Timers.Timer timerToDisableRespawn = AddTimer(Config.RespawnTime, () => {
                Server.PrintToConsole($"{ Config.ChatPrefix } {Config.RespawnTime} seconds has been passed since round Start. Turning Off Respawn...");
                canRespawn = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

    private void Respawn(CCSPlayerController? player)
    {
        if (player == null) return;

        if (player.PawnIsAlive)
        {
            Server.PrintToConsole($"{ Config.ChatPrefix } It's not possible to revive the player \"{ player.PlayerName }\", he's already alive.");
            return;
        }

        VirtualFunction.CreateVoid<CCSPlayerController>(player.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(player);
    }
}