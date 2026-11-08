namespace TradeMonitor.Contracts;

// Hər fayl oxuyucu (plugin) bu interface-i həyata keçirir.
// Name: appsettings.json-da loader-i aktiv etmək üçün ad (məs. "CSV").
// FileExtension: oxuduğu fayl uzantısı (məs. ".csv").
// Load: faylı oxuyur və bütün ticarət sətirlərini qaytarır.
// Yeni format üçün DLL yazıb "Plugins" qovluğuna atmaq kifayətdir, proqramı dəyişmək lazım deyil.
public interface ITradeLoader
{
    string Name { get; }

    string FileExtension { get; }

    List<Trade> Load(string filePath);
}
