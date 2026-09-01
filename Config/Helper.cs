using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Translations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Auto_Delete_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using System.IO.Enumeration;

namespace Auto_Delete_GoldKingZ;

public class Helper
{
    static string? _csgoRoot;
    static string? _gameRoot;
    public static string CsgoRoot => _csgoRoot ??= FindCsgo();
    public static string GameRoot => _gameRoot ??= CsgoRoot == "" ? "" : Path.GetFullPath(Path.Combine(CsgoRoot, ".."));
    static bool IsCsgo(string p) => !string.IsNullOrWhiteSpace(p) && Directory.Exists(Path.Combine(p, "addons")) && File.Exists(Path.Combine(p, "gameinfo.gi"));
    public static bool HasPattern(List<string>? patterns) => patterns != null && patterns.Any(p => !string.IsNullOrWhiteSpace(p));

    public static void RegisterCssCommands(string[]? commands, string description, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.AddCommand(cmd, description, callback);
        }
    }


    public static void RemoveCssCommands(string[]? commands, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.RemoveCommand(cmd, callback);
        }
    }

    public static void RegisterCssListener(string[]? commands, CommandInfo.CommandListenerCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.AddCommandListener(cmd, callback, HookMode.Pre);
        }
    }

    public static void RemoveCssListener(string[]? commands, CommandInfo.CommandListenerCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.RemoveCommandListener(cmd, callback, HookMode.Pre);
        }
    }

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, CounterStrikeSharp.API.Modules.Commands.CommandInfo commandInfo, string message, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i]?.ToString() ?? "");
        }

        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
                    {
                        player.PrintToConsole(" " + trimmedPart);
                    }
                    else
                    {
                        player.PrintToChat(" " + trimmedPart);
                    }
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
            {
                player.PrintToConsole(message);
            }
            else
            {
                player.PrintToChat(message);
            }
        }
    }

    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString() ?? "");
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString() ?? "");
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }

    //----
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrEmpty(groups) || player == null || !player.IsValid)
            return false;

        return groups.Split('|')
            .Select(segment => segment.Trim())
            .Any(trimmedSegment => Permission_CheckPermissionSegment(player, trimmedSegment));
    }

    private static bool Permission_CheckPermissionSegment(CCSPlayerController player, string segment)
    {
        if (string.IsNullOrEmpty(segment)) return false;

        int colonIndex = segment.IndexOf(':');
        if (colonIndex == -1 || colonIndex == 0) return false;

        string prefix = segment.Substring(0, colonIndex).Trim().ToLower();
        string values = segment.Substring(colonIndex + 1).Trim();

        return prefix switch
        {
            "steamid" or "steamids" or "steam" or "steams" => Permission_CheckSteamIds(player, values),
            "flag" or "flags" => Permission_CheckFlags(player, values),
            "group" or "groups" => Permission_CheckGroups(player, values),
            _ => false
        };
    }

    private static bool Permission_CheckSteamIds(CCSPlayerController player, string steamIds)
    {
        if (string.IsNullOrEmpty(steamIds)) return false;

        steamIds = steamIds.Replace("[", "").Replace("]", "");

        var (steam2, steam3, steam32, steam64) = player.SteamID.GetPlayerSteamID();
        var steam3NoBrackets = steam3.Trim('[', ']');

        return steamIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .Any(trimmedId =>
                string.Equals(trimmedId, steam2, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam3NoBrackets, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam32, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam64, StringComparison.OrdinalIgnoreCase)
            );
    }

    private static bool Permission_CheckFlags(CCSPlayerController player, string flags)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(flags))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null)
            return false;

        var requiredFlags = flags
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToList();

        if (playerData._flags != null &&
            requiredFlags.Any(reqFlag =>
                playerData._flags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase)))
            return true;

        var allFlags = playerData.GetAllFlags();
        return allFlags != null &&
            requiredFlags.Any(reqFlag =>
                allFlags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase));
    }

    private static bool Permission_CheckGroups(CCSPlayerController player, string groups)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(groups))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null || playerData.Groups == null || !playerData.Groups.Any())
            return false;

        return groups
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Any(reqGroup => playerData.Groups.Contains(reqGroup, StringComparer.OrdinalIgnoreCase));
    }

    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        try
        {
            return Utilities.GetPlayers()
                .Where(p =>
                    (IncludeBots || !p.IsBot) &&
                    (IncludeHLTV || !p.IsHLTV) &&
                    ((IncludeCT   && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
                    (IncludeT    && p.TeamNum == (byte)CsTeam.Terrorist) ||
                    (IncludeNone && p.TeamNum == (byte)CsTeam.None) ||
                    (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
                .ToList();
        }
        catch (NativeException)
        {
            return new();
        }
    }

    public static void ReloadPlayersGlobals()
    {
        foreach (var players in GetPlayersController())
        {
            if(!players.IsValid()) continue;

            CheckPlayerInGlobals(players);
        }
    }
    public static void CheckPlayerInGlobals(CCSPlayerController player)
    {
        if(!player.IsValid()) return;

        var g_Main = MainPlugin.Instance.g_Main;
        if (!g_Main.Player_Data.ContainsKey(player.Slot))
        {
            var initialData = new Globals.PlayerDataClass(
                player,
                DateTime.MinValue
            );
            g_Main.Player_Data.TryAdd(player.Slot, initialData);
        }else
        {
            g_Main.Player_Data[player.Slot].Player = player;
        }
    }

    public static void ClearVariables()
    {
        var g_Main = MainPlugin.Instance.g_Main;

        g_Main.Clear();
    }

    public static void DebugMessage(string message, bool important = false, Con? prefixColor = null)
    {
        const string prefix = "[Auto Delete]";
        if (!Configs.Instance.EnableDebug && !important) return;
        Con defaultColor = important ? Con.Red : Con.Magenta;
        prefixColor ??= Con.Purple;
        Con.WriteLine($"{prefixColor}{prefix}: {defaultColor}{message}{Con.Reset}");
    }

    public static void MuteCommands(CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um, int Config, bool Fully = false)
    {
        if (um == null) return;
        if ((!Fully && Config > 0) || (Fully && Config == 2))
        {
            um.Recipients.Clear();
        }
    }

    static string FindCsgo()
    {
        string dir = Server.GameDirectory ?? "";
 
        foreach (string p in new[] { dir, Path.Combine(dir, "csgo") })
            if (IsCsgo(p)) return Path.GetFullPath(p);
 
        for (var d = new DirectoryInfo(Application.RootDirectory); d != null; d = d.Parent)
            if (IsCsgo(d.FullName)) return d.FullName;
 
        DebugMessage("Cant Find csgo Folder, All Paths Skipped", true);
        return "";
    }

    public static void RunCleanup()
    {
        foreach (var entry in Configs.ValidPaths())
        {
            string full   = entry.Full_Path;
            bool   single = File.Exists(full);

            if (!single && !Directory.Exists(full))
            {
                DebugMessage($"{Con.Gold}---------[ {Con.Cyan}{entry.Path}{Con.Gold} ] [ {Con.Gray}Does Not Exist{Con.Gold} ]---------{Con.Reset}", false);
                continue;
            }

            var lines   = new List<(string Text, bool Important)>();
            int deleted = 0;

            void Process(string item, bool isFolder)
            {
                if (!IsPathAllowed(item)) return;

                double days = (DateTime.UtcNow - (isFolder ? Directory.GetLastWriteTimeUtc(item) : File.GetLastWriteTimeUtc(item))).TotalDays;
                string what = isFolder ? "Folder" : "File";
                string info = $"{Con.Silver}{ShortPath(item)}{(isFolder ? Path.DirectorySeparatorChar.ToString() : "")} {Con.Gray}(Last Edit: {AgeText(days)}){Con.Reset}";

                if (entry.Days > 0 && days < entry.Days)
                {
                    lines.Add(($"{Con.Red}[Too New]{Con.Reset} Skipped {what} {info} {Con.Gray}(OlderThanXDays: {entry.Days}){Con.Reset}", false));
                    return;
                }

                try
                {
                    if (isFolder) Directory.Delete(item, true);
                    else File.Delete(item);

                    deleted++;
                    lines.Add(($"{Con.Green}[Success]{Con.Reset} Deleted {what} {info}", true));
                }
                catch (Exception ex)
                {
                    lines.Add(($"{Con.Crimson}[Failed]{Con.Reset} {what} {info} {Con.Gray}| {ex.Message}{Con.Reset}", true));
                }
            }

            if (single)
            {
                Process(full, false);
            }
            else
            {
                if (HasPattern(entry.Files))
                    foreach (string file in Directory.EnumerateFiles(full).Where(f => MatchesAny(Path.GetFileName(f), entry.Files)).OrderByDescending(File.GetLastWriteTimeUtc))
                        Process(file, false);

                if (HasPattern(entry.Folders))
                    foreach (string folder in Directory.EnumerateDirectories(full).Where(d => MatchesAny(Path.GetFileName(d), entry.Folders)).OrderByDescending(Directory.GetLastWriteTimeUtc))
                        Process(folder, true);
            }

            if (lines.Count == 0)
            {
                DebugMessage($"{Con.Gold}---------[ {Con.Cyan}{entry.Path}{Con.Gold} ] [ {Con.Gray}Nothing To Delete{Con.Gold} ]---------{Con.Reset}", false);
                continue;
            }

            DebugMessage($"{Con.Gold}---------[ {Con.Cyan}{entry.Path}{Con.Gold} ] [ {Con.Green}{deleted}{Con.Gold} Deleted ]---------{Con.Reset}", deleted > 0);

            foreach (var (text, important) in lines) DebugMessage(text, important);
        }
    }
 
    static string AgeText(double days) =>
        days < 1 ? "Today" : $"{(int)days} Day{((int)days == 1 ? "" : "s")} Ago";
 
    static string ShortPath(string file)
    {
        string dir = Path.GetFileName(Path.GetDirectoryName(file) ?? "");
 
        return dir.Length == 0 ? Path.GetFileName(file) : $"...{Path.DirectorySeparatorChar}{dir}{Path.DirectorySeparatorChar}{Path.GetFileName(file)}";
    }
    
    public static string ResolvePath(string raw)
    {
        if (GameRoot == "" || string.IsNullOrWhiteSpace(raw)) return "";

        string p = raw.Replace('\\', '/').Trim();
        if (p.Length == 0) return "";

        p = Regex.Replace(p, @"\{back\}", "..", RegexOptions.IgnoreCase);

        return Path.GetFullPath(Path.Combine(GameRoot, p));
    }
 
    public static bool IsPathAllowed(string fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath)) return false;
 
        string p = Path.GetFullPath(fullPath).TrimEnd('/', '\\');
 
        if (p.Length == 0) return false;
        if (p.Equals(Path.GetPathRoot(p)?.TrimEnd('/', '\\'), StringComparison.OrdinalIgnoreCase)) return false;
        if (p.Equals(GameRoot.TrimEnd('/', '\\'), StringComparison.OrdinalIgnoreCase)) return false;
        if (p.Equals(CsgoRoot.TrimEnd('/', '\\'), StringComparison.OrdinalIgnoreCase)) return false;
 
        return true;
    }
 
    public static bool MatchesAny(string fileName, List<string>? patterns)
    {
        if (patterns == null) return false;
 
        foreach (string pattern in patterns)
            if (!string.IsNullOrWhiteSpace(pattern) && FileSystemName.MatchesSimpleExpression(pattern, fileName, ignoreCase: true)) return true;
 
        return false;
    }


    public static void RegisterCommandsAndHooks()
    {
        MainPlugin.Instance.RegisterListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RegisterListener<Listeners.OnClientPutInServer>(MainPlugin.Instance.OnClientPutInServer);
        MainPlugin.Instance.RegisterListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.AddCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.AddCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.HookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RegisterCssCommands(Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(), "Commands To Reload Auto Delete Files Plugin", MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
    }

    public static void RemoveRegisterCommandsAndHooks()
    {
        MainPlugin.Instance.RemoveListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RemoveListener<Listeners.OnClientPutInServer>(MainPlugin.Instance.OnClientPutInServer);
        MainPlugin.Instance.RemoveListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.RemoveCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.RemoveCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.UnhookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RemoveCssCommands(Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
    }
}