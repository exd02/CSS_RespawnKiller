using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

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
        if (player == null) return HookResult.Continue;

        if (canRespawn)
        {
            AddTimer(0.5f, () => {
                Respawn(player);
            });
        }

        AddTimer(0.1f, () => {
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
            PrintColoredAll(Localizer["rk.auto.detection.disable.respawn"]);
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

        // It's better to exec necessary commands here than onMapStart, because we'll make sure that we overwrite other configs.
        if (!bExecMapCfg)
        {
            if (Config.LetPluginDecideForRoundEndConditions)
                Server.ExecuteCommand("mp_ignore_round_win_conditions true");
            
            Server.ExecuteCommand("mp_respawn_on_death_t 0");
            Server.ExecuteCommand("mp_respawn_on_death_ct 0");
            
            if (File.Exists(GetCurrentMapConfigPath()))
            {
                PrintConDebug("First round detected, changing server commands!");
                
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
                PrintColoredAll(Localizer["rk.timer.disable.respawn", respawnTime]);
                canRespawn = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }

}