using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Auto_Delete_Logs;

public class AutoDeleteLogsConfig : BasePluginConfig
{
    [JsonPropertyName("CounterstrikeSharpMoreThanXdaysOld")] public int CounterstrikeSharpMoreThanXdaysOld { get; set; } = 0;
    [JsonPropertyName("BackupRoundMoreThanXdaysOld")] public int BackupRoundMoreThanXdaysOld { get; set; } = 0;
    [JsonPropertyName("DemoMoreThanXdaysOld")] public int DemoMoreThanXdaysOld { get; set; } = 0;
}

public class AutoDeleteLogs : BasePlugin, IPluginConfig<AutoDeleteLogsConfig>
{
    public override string ModuleName => "Auto Delete Logs";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Auto Delete Logs Files";
    public AutoDeleteLogsConfig Config { get; set; } = new AutoDeleteLogsConfig();
    public void OnConfigParsed(AutoDeleteLogsConfig config)
    {
        Config = config;
    }
    
    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
    }
    private void OnMapStart(string Map)
	{
		if(Config.CounterstrikeSharpMoreThanXdaysOld > 0)
        {
            DeleteCounterstrikeSharp();
        }

        if(Config.BackupRoundMoreThanXdaysOld > 0)
        {
            DeleteBackUP();
        }

        if(Config.DemoMoreThanXdaysOld > 0)
        {
            DeleteDemos();
        }
	}

    private void DeleteCounterstrikeSharp()
    {
        try
        {
            string folderPath = Path.Combine(ModuleDirectory, "../../plugins");
            for (int i = 0; i < 1; i++)
            {
                folderPath = Path.Combine(folderPath, "..");
            }
            string LOGS = Path.Combine(folderPath, "logs");

            if (Directory.Exists(LOGS))
            {
                string[] files = Directory.GetFiles(LOGS);
                DateTime cutoffDate = DateTime.Now.AddDays(-Config.CounterstrikeSharpMoreThanXdaysOld);

                foreach (string file in files)
                {
                    try
                    {
                        DateTime lastWriteTime = File.GetLastWriteTime(file);

                        if (lastWriteTime < cutoffDate)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing directory: {ex.Message}");
        }
    }
    private void DeleteBackUP()
    {
        try
        {
            string folderPath = Path.Combine(ModuleDirectory, "../../plugins");
            for (int i = 0; i < 3; i++)
            {
                folderPath = Path.Combine(folderPath, "..");
            }

            string[] files = Directory.GetFiles(folderPath, "backup_round*.txt");

            DateTime cutoffDate = DateTime.Now.AddDays(-Config.BackupRoundMoreThanXdaysOld);

            foreach (string file in files)
            {
                try
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(file);

                    if (lastWriteTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing directory: {ex.Message}");
        }
    }
    private void DeleteDemos()
    {
        try
        {
            string folderPath = Path.Combine(ModuleDirectory, "../../plugins");

            for (int i = 0; i < 3; i++)
            {
                folderPath = Path.Combine(folderPath, "..");
            }

            string[] files = Directory.GetFiles(folderPath, "*.dem");

            DateTime cutoffDate = DateTime.Now.AddDays(-Config.DemoMoreThanXdaysOld);

            foreach (string file in files)
            {
                try
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(file);

                    if (lastWriteTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing directory: {ex.Message}");
        }
    }
}