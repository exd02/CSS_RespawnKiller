using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Commands;

namespace RespawnKiller;

public partial class RespawnKiller
{
	public void InitializeEvents()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath,HookMode.Post);

        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        AddCommandListener("jointeam", OnJoinTeam);
    }

    public HookResult OnJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
	{
        AddTimer(1.0f, () => {
            CheckForRoundEndConditions();
        });

		return HookResult.Continue;
	}

    private void OnMapStart(string mapName)
    {
        Server.NextFrame(() =>
        {
            AddTimer(5.0f, () => {
                PrintConDebug($"MapStarted, cleaning variables...");
                ResetVars(); 
            });
        });
    }

    HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (!canRespawn)
        {
            // the player is alive so we add a timer
            AddTimer(1.0f, () => {
                CheckForRoundEndConditions();
            });
            return HookResult.Continue;
        }

        CCSPlayerController? player = @event.Userid;

        if (player == null) return HookResult.Continue;
        
        double thisDeathTime = Server.EngineTime;
        double deltaDeath = thisDeathTime - lastDeathTime[player.Slot];
        lastDeathTime[player.Slot] = thisDeathTime;
        
        if (deltaDeath < 0)
        {
            PrintConDebug($"CRITICAL: Delta death is negative!!!");
            return HookResult.Continue;
        }

        if (deltaDeath < Config.TimeBtwPlayerDeathsToDetectRespawnKill && autoDetectRespawnKill)
        {
            PrintColoredAll($"Auto-Detection has detected Respawn-Kill, disabling respawn!");
            canRespawn = false;

            AddTimer(1.0f, () => {
                CheckForRoundEndConditions();
            });
            
        }

        if (canRespawn)
        {
            PrintConDebug($"Respawn is still active, spawning player \"{ player.PlayerName }\" in {Config.TimeDeadScreen} seconds!");
            
            AddTimer(Config.TimeDeadScreen, () => { Respawn(player); }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
        AddTimer(1.0f, () => {
            CheckForRoundEndConditions();
        });
        
        CCSPlayerController? player = @event.Userid;

        if (!player.IsValid)
        {
            return HookResult.Continue;
        }


        PrintConDebug($"Player \"{ player.PlayerName }\" (slot { player.Slot }) has disconnected, setting lastDeathTime[{ player.Slot }] = 0.");

        lastDeathTime[player.Slot] = 0.0;

        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnEventRoundStartPre(EventRoundStart @event, GameEventInfo info)
    {
        if (timerToDisableRespawn != null)
            timerToDisableRespawn.Kill();

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
            timerToDisableRespawn = AddTimer(respawnTime, () => {
                PrintColoredAll($"{respawnTime} seconds has been passed since round start, turning off respawn.");
                canRespawn = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

}