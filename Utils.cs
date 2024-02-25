using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

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
        
        player.PrintToChat($"\x10{ Config.ChatPrefix }\x08 { message }");
    }
    
    private void PrintColoredAll(string message)
    {
        string coloredMsg = $"\x10{ Config.ChatPrefix }\x08 { message }";

        Server.PrintToConsole(coloredMsg);
        Server.PrintToChatAll(coloredMsg);
    }
}