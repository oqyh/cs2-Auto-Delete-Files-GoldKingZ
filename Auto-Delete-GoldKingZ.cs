using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using Auto_Delete_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Text;

namespace Auto_Delete_GoldKingZ;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "[Auto Delete] Auto Delete Any Files And Folders From Any Given Paths On Every Map Change";
    public override string ModuleVersion => "1.0.4";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    public readonly Game_UserMessages Game_UserMessages = new();
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory, hotReload);

        Helper.RemoveRegisterCommandsAndHooks();
        Helper.ClearVariables();
        Helper.RegisterCommandsAndHooks();
        Helper.ReloadPlayersGlobals();
        Helper.RunCleanup();
    }

    public void OnMapStart(string mapname)
    {
        Helper.RunCleanup();
    }

    public void OnClientPutInServer(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if(!player.IsValid())return;

        Helper.CheckPlayerInGlobals(player);
    }

    public HookResult OnPlayerSay(CCSPlayerController? player, CommandInfo info)
    {
        return HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnPlayerSay_Team(CCSPlayerController? player, CommandInfo info)
    {
        return HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnUserMessage_OnSayText2(CounterStrikeSharp.API.Modules.UserMessages.UserMessage um)
    {
        var player = Utilities.GetPlayerFromIndex(um.ReadInt("entityindex"));
        return HandlePlayerMessage(player, Encoding.UTF8.GetString(um.ReadBytes("param2")), um);
    }

    private HookResult HandlePlayerMessage(CCSPlayerController? player, string? rawMessage, CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um = null)
    {
        if (!player.IsValid() || string.IsNullOrWhiteSpace(rawMessage)) return HookResult.Continue;

        string message = rawMessage.Trim();
        Game_UserMessages.HookPlayerChat_UserMessages(player, message, um);

        return HookResult.Continue;
    }

    public void OnMapEnd()
    {
        try
        {
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"OnMapEnd Error: {ex.Message}", true);
        }
    }

    public override void Unload(bool hotReload)
    {
        try
        {
            Helper.RemoveRegisterCommandsAndHooks();
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Unload Error: {ex.Message}", true);
        }
    }

    /* 
    [ConsoleCommand("css_test", "test")]
    public void tesstttt(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(player == null || !player.IsValid)return;

    } */
}