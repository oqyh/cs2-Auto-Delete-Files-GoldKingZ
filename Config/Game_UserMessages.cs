using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text;
using Auto_Delete_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using Newtonsoft.Json.Linq;
using CounterStrikeSharp.API.Modules.Commands.Targeting;

namespace Auto_Delete_GoldKingZ;

public class Game_UserMessages
{
    public HookResult HookPlayerChat_UserMessages(CCSPlayerController? player, string message, UserMessage? um = null)
    {
        if(!player.IsValid()) return HookResult.Continue;

        if (Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
        {
            Handle_ReloadPlugin(player, null!, um!);
        }

        
        return HookResult.Continue;
    }

    #region Commands Hook

    public void CommandsAction_ReloadPlugin(CCSPlayerController? player, CommandInfo info)
    {
        if(!player.IsValid()) return;

        Handle_ReloadPlugin(player, info, null!);
    }

    

    #endregion Commands Hook
    



    #region Handles

    public static void Handle_ReloadPlugin(CCSPlayerController player, CommandInfo commandInfo = null!, UserMessage um = null!)
    {
        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;

        bool onetime = (DateTime.Now - playerData.EventPlayerChat).TotalSeconds > 0.4;
        if (onetime) playerData.EventPlayerChat = DateTime.Now;


        var cfg = Configs.Instance.Reload_Plugin;

        if (cfg.Reload_Plugin_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, cfg.Reload_Plugin_Flags))
        {
            if (onetime)
            {
                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Not.Allowed"]);
            }
        }
        else
        {
            if (onetime)
            {
                Server.NextFrame(() =>
                {
                    Helper.RemoveRegisterCommandsAndHooks();
                    Helper.ClearVariables();
                    Configs.Load(MainPlugin.Instance.ModuleDirectory, true);
                    Helper.RegisterCommandsAndHooks();
                    Helper.ReloadPlayersGlobals();
                    Helper.RunCleanup();
                });

                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Successfully"]);
            }

            Helper.MuteCommands(um, cfg.Reload_Plugin_Hide);
        }

        Helper.MuteCommands(um, cfg.Reload_Plugin_Hide, true);
    }
    
    #endregion Handles
}