using System.IO;
using System.Text.Json;

namespace TradeMonitor.App;

// appsettings.json faylından (TradeMonitor.exe-nin yanında) oxunan ayarlar:
// InputFolder - izlənən qovluq, CheckIntervalSeconds - neçə saniyədən bir yoxlanır,
// PluginFolder - plugin DLL-lərinin qovluğu, EnabledLoaders - aktiv loader-lərin adları.
// Ayarı dəyişmək üçün faylı redaktə edib proqramı yenidən açmaq kifayətdir.
public class AppSettings
{
    public string InputFolder { get; set; } = "Input";

    public int CheckIntervalSeconds { get; set; } = 5;

    public string PluginFolder { get; set; } = "Plugins";

    public List<string> EnabledLoaders { get; set; } = new();

    // appsettings.json-u oxuyub AppSettings obyektinə çevirir.
    // Nisbi yolları ("Input") proqram qovluğuna görə tam yola çevirir; tam yol olduğu kimi qalır.
    public static AppSettings Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        string json = File.ReadAllText(path);
        AppSettings settings = JsonSerializer.Deserialize<AppSettings>(json)!;

        settings.InputFolder = Path.Combine(AppContext.BaseDirectory, settings.InputFolder);
        settings.PluginFolder = Path.Combine(AppContext.BaseDirectory, settings.PluginFolder);

        return settings;
    }
}
