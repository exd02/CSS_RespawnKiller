using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using System.Text.Json;

namespace RespawnKiller;

public partial class RespawnKiller
{
    private void Respawn(CCSPlayerController? player)
    {
        if (player == null) return;

        if (player.PawnIsAlive)
        {
            if (Config.DebugMessages)
                PrintColored($"It's not possible to revive the player \"{ player.PlayerName }\", he's already alive.", player);
            
            return;
        }

        VirtualFunction.CreateVoid<CCSPlayerController>(player.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(player);
    }

    // print colored to the player in private chat. if player is null print only in server console
    private void PrintColored(string message, CCSPlayerController? player = null)
    {
        // print to the server, if the command is whispered to a player print who is the player
        Server.PrintToConsole($"{ Config.ChatPrefix }{(player != null ? $" [{player.PlayerName}] ->" : "")} { message }");

        if (player == null)
        {
            return;
        }
        
        player.PrintToChat($"\u0002{ Config.ChatPrefix }\u0001 { message }");
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
    
    private void PrintColoredAll(string message)
    {
        string coloredMsg = $"\u0002{ Config.ChatPrefix }\u0001 { message }";

        Server.PrintToConsole(coloredMsg);
        Server.PrintToChatAll(coloredMsg);
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
            PrintColored($"Could not find cfg directory in \"{path})\"");
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

        PrintColoredAll($"New map configs writted, restarting round.");
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
                    PrintColored($"\"{path}\" was null.");
                    return;
                }
                
                autoDetectRespawnKill = mapSettings.AutoDetection;
                respawnTime = mapSettings.RespawnTime;

            }
        }
        catch (Exception ex)
        {
            PrintColored($"Could not load \"{path}\"! Error: {ex.Message}");
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
                PrintColored($"Could not parse \"{path}\"! Error: {ex.Message}");
            }
        }

        return null;
    }
}
