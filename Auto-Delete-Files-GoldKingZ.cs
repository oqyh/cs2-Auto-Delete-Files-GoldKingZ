using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using Newtonsoft.Json.Linq;
using Auto_Delete_Files_GoldKingZ.Config;

namespace Auto_Delete_Files_GoldKingZ;

public class AutoDeleteFilesGoldKingZ : BasePlugin
{
    public override string ModuleName => "Auto Delete Files";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	
	

    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
    }
    private void OnMapStart(string Map)
    {
        try
        {
            string jsonFilePath = Path.Combine(ModuleDirectory, "../../plugins/Auto-Delete-Files-GoldKingZ/config/AutoDelete_Settings.json");
            string jsonData = File.ReadAllText(jsonFilePath);
            JObject jsonObject = JObject.Parse(jsonData);
            if (jsonObject == null) return;

            var itemKeys = jsonObject.Properties()
                                    .Where(p => p.Name.StartsWith("AutoDelete_"))
                                    .Select(p => p.Name)
                                    .ToList();

            bool startsWithItem = jsonObject.Properties()
                                    .Any(p => p.Name.StartsWith("AutoDelete_"));

            if(startsWithItem)
            {
                foreach (var key in itemKeys)
                {
                    var itemMenu = jsonObject[key];
                    if (itemMenu != null)
                    {
                        JObject deleteData = itemMenu.Value<JObject>()!;

                        string deletePath = deleteData["Delete_Path"]!.ToString();
                        string deleteFiles = deleteData["Delete_Files"]!.ToString();
                        int deleteOlderThanXDays = (int)deleteData["Delete_OlderThanXDays"]!;

                        deletePath = Path.Combine(Server.GameDirectory, deletePath);

                        if (!Directory.Exists(deletePath))
                        {
                            if(Configs.GetConfigData().SendErrorLogsToServerConsole)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"================================================================  E  R  R  O  R ================================================================");
                                Console.WriteLine($"[Auto Delete Files Gold KingZ] Directory does not exist: {deletePath}");
                                Console.WriteLine($"================================================================  E  R  R  O  R ================================================================");
                                Console.ResetColor();
                            }
                            continue;
                        }
                        DateTime currentDate = DateTime.Now;
                        DateTime thresholdDate = currentDate.AddDays(-deleteOlderThanXDays);

                        string[] files = Directory.GetFiles(deletePath, deleteFiles);

                        if(Configs.GetConfigData().SendErrorLogsToServerConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine($"================================================================ D E L E T I N G  ================================================================");
                            Console.ResetColor();
                        }
                        foreach (var file in files)
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.LastWriteTime < thresholdDate)
                            {
                                fileInfo.Delete();
                                if(Configs.GetConfigData().SendErrorLogsToServerConsole)
                                {
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.WriteLine($"[Auto Delete Files Gold KingZ] Deleted File: {fileInfo.FullName}");
                                    Console.ResetColor();
                                }
                                
                            }
                        }
                        if(Configs.GetConfigData().SendErrorLogsToServerConsole)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine($"================================================================ D E L E T I N G  ================================================================");
                            Console.ResetColor();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if(Configs.GetConfigData().SendErrorLogsToServerConsole)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"================================================================  E  R  R  O  R ================================================================");
                Console.WriteLine($"[Auto Delete Files Gold KingZ] An error occurred: {ex.Message}");
                Console.WriteLine($"================================================================  E  R  R  O  R ================================================================");
                Console.ResetColor();
            }
            
        }
    }
}