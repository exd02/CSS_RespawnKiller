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
            // example: acrophobia 60 seconds, auto detect 0

            PrintColored($"MapStarted, cleaning lastDeathTime...");

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
            PrintColoredAll($"Auto Respawn Kill Detection has been activated!");
            canRespawn = false;
        }

        if (canRespawn)
        {
            if (Config.DebugMessages)
                PrintColored($"Respawn active, spawning player \"{ player.PlayerName }\" in {Config.TimeDeadScreen} seconds!", player);
            
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

        if (Config.DebugMessages)
            PrintColored($"Player \"{ player.PlayerName }\" (slot { player.Slot }) has disconnected, setting lastDeathTime[{ player.Slot }] = 0.");

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
                PrintColoredAll($"{Config.RespawnTime} seconds has been passed since round start, turning off respawn.");
                canRespawn = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

}