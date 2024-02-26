using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;

namespace RespawnKiller;

public partial class RespawnKiller
{
    private void Respawn(CCSPlayerController? player)
    {
        if (player == null) return;

        if (player.PawnIsAlive)
        {
            PrintConDebug($"It's not possible to revive the player \"{ player.PlayerName }\", he's already alive.");
            return;
        }

        VirtualFunction.CreateVoid<CCSPlayerController>(player.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(player);
    }

    private void PrintConError(string message)
    {
        Server.PrintToConsole($"\u001b[36m{ Config.ChatPrefix } \u001B[31m[ERROR] \u001B[37m{ message }");
    }

    private void PrintConDebug(string message)
    {
        if (Config.DebugMessages)
            Server.PrintToConsole($"\u001b[36m{ Config.ChatPrefix } [DEBUG] \u001b[37m{message}");
    }

    private void PrintColoredAll(string message)
    {
        Server.PrintToConsole($"\u001b[36m{ Config.ChatPrefix } \u001b[37m{message}");
        Server.PrintToChatAll($" \x05 {Config.ChatPrefix} \x01 {message}");
    }

    private void PrintColored(string message, CCSPlayerController? player = null)
    {
        // print to the server, if the command is whispered to a player print who is the player
        Server.PrintToConsole($"\u001b[36m{ Config.ChatPrefix } \u001b[37m{(player != null ? $" [{player.PlayerName}] ->" : "")} { message }");

        if (player == null)
        {
            return;
        }
        
        player.PrintToChat($" \x05 {Config.ChatPrefix} \x01  { message }");
    }

    private int CountAlivePlayers()
    {
        int count = 0;
        foreach (CCSPlayerController? player in Utilities.GetPlayers())
        {
            if (player != null && player.IsValid && player.PawnIsAlive && !player.IsBot)
            {
                count++;
            }
        }
        return count;
    }

    // count
    private int CountInPlayingTeamPlayers()
    {
        int count = 0;
        foreach (CCSPlayerController? player in Utilities.GetPlayers())
        {
            if (player != null && player.IsValid && player.TeamNum != (byte)CsTeam.None)
            {
                count++;
            }
        }
        return count;
    }

    private void CheckForRoundEndConditions()
    {
        if (!Config.LetPluginDecideForRoundEndConditions || canRespawn)
            return;

        int alive = CountAlivePlayers();
        int playing = CountInPlayingTeamPlayers();

        if (alive == 0 && playing > 0)
        {
            PrintConDebug($"RoundEndConditions Confirmed -> Alive = {alive}, Playing (CT/T): {playing}.");
            Server.ExecuteCommand("mp_restartgame 1");
        }
    }

    private void ResetVars()
    {
        for (int i = 0; i < 64; i++)
        {
            lastDeathTime[i] = 0.0;
        }
        
        bExecMapCfg = false;
        autoDetectRespawnKill = true;
        respawnTime = 0.0f;
    }

    public string GetCurrentMapConfigPath()
    {
        return $"{gameDir}/csgo/cfg/RespawnKiller/MapSettings/{Server.MapName}.json";
    }

    public void ValidateMapSettingsFolder()
    {
        string path = $"{gameDir}/csgo/cfg";

        if (!Directory.Exists(path))
        {
            PrintConError($"Could not find cfg directory in \"{path})\"");
            return;
        }

        path += "/RespawnKiller";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path += "/MapSettings";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void SaveMapConfig(bool autoDetection, float respawnTime)
    {        
        MapSettings mapSettings = new MapSettings
        {
            AutoDetection = autoDetection,
            RespawnTime = respawnTime
        };

        string newJson = JsonSerializer.Serialize(mapSettings, new JsonSerializerOptions { WriteIndented = true });
        
        string path = GetCurrentMapConfigPath();
        File.WriteAllText(path, newJson);

        PrintColoredAll(Localizer["rk.restart.new.config"]); 
        Server.ExecuteCommand("mp_restartgame 1");
    }

    public void LoadMapConfig()
    {
        string path = GetCurrentMapConfigPath();

        try
        {
            using (JsonDocument jsonDocument = LoadJson(path))
            {
                if (jsonDocument == null)
                {
                    return;
                }

                var mapSettings = JsonSerializer.Deserialize<MapSettings>(jsonDocument.RootElement.GetRawText());
                
                if (mapSettings == null)
                {
                    PrintConError($"\"{path}\" was null.");
                    return;
                }
                
                autoDetectRespawnKill = mapSettings.AutoDetection;
                respawnTime = mapSettings.RespawnTime;

            }
        }
        catch (Exception ex)
        {
            PrintConError($"Could not load \"{path}\"! Error: {ex.Message}");
        }
    }

    private JsonDocument? LoadJson(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                PrintConError($"Could not parse \"{path}\"! Error: {ex.Message}");
            }
        }

        return null;
    }
}
