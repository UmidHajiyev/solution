using System.IO;
using System.Reflection;
using TradeMonitor.Contracts;

namespace TradeMonitor.App;

// Loader plugin-lərini proqram işləyərkən tapır.
public static class PluginLoader
{
    // Plugins qovluğundakı hər DLL-i açır, içində ITradeLoader-i həyata keçirən class-ları tapır
    // və onlardan obyekt yaradır (Reflection). Yalnız appsettings.json-da adı olan loader-lər götürülür.
    // Nəticə: fayl uzantısı -> loader, məsələn ".csv" -> CsvTradeLoader.
    // Səhv DLL olsa, keçilir ki, bütün proqram dayanmasın.
    public static Dictionary<string, ITradeLoader> LoadLoaders(string pluginFolder, List<string> enabledLoaders)
    {
        var loaders = new Dictionary<string, ITradeLoader>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(pluginFolder))
        {
            return loaders;
        }

        foreach (string dllPath in Directory.GetFiles(pluginFolder, "*.dll"))
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);

                foreach (Type type in assembly.GetTypes())
                {
                    bool isLoader = typeof(ITradeLoader).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;
                    if (!isLoader)
                    {
                        continue;
                    }

                    var loader = (ITradeLoader)Activator.CreateInstance(type)!;

                    if (enabledLoaders.Contains(loader.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        loaders[loader.FileExtension] = loader;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        return loaders;
    }
}
