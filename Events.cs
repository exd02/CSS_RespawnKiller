using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;

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
            AddTimer(5.0f, () => {
                PrintColored($"MapStarted, cleaning variables...");
                ResetVars(); 
            });
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

        if (timeAlive < 5 && autoDetectRespawnKill)
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
        // I tried to do this inside OnMapStart but that shit is crashing even with a timer
        if (!bExecMapCfg)
        {
            if (File.Exists(GetCurrentMapConfigPath()))
            {
                LoadMapConfig();
                bExecMapCfg = true;
            }
        }

        // Change the respawn variable back to on
        canRespawn = true;

        // Create a timer to set the respawn variable to false
        if (respawnTime > 0.0)
        {
            // 'Timer' is an ambiguous reference between 'CounterStrikeSharp.API.Modules.Timers.Timer' and 'System.Threading.Timer'
            CounterStrikeSharp.API.Modules.Timers.Timer timerToDisableRespawn = AddTimer(respawnTime, () => {
                PrintColoredAll($"{respawnTime} seconds has been passed since round start, turning off respawn.");
                canRespawn = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

}

/*



*/