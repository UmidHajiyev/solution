using TradeMonitor.Contracts;

namespace TradeMonitor.Loaders;

// .csv fayllarını oxuyur: dəyərlər vergüllə ayrılıb, başlıq sətri yoxdur.
// Nümunə sətir: 2013-5-20,30.16,30.39,30.02,30.17,1478200
public class CsvTradeLoader : ITradeLoader
{
    public string Name => "CSV";

    public string FileExtension => ".csv";

    // Faylı sətir-sətir oxuyur (böyük fayl yaddaşı doldurmur), boş sətirləri keçir,
    // hər sətri vergülə görə 6 hissəyə bölüb Trade-ə çevirir.
    // 6 dəyər olmayan sətir varsa, xəta atır və ekranda "ERROR" görünür.
    public List<Trade> Load(string filePath)
    {
        var trades = new List<Trade>();

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            if (parts.Length != 6)
            {
                throw new FormatException($"Line \"{line}\" should have 6 values separated by commas.");
            }

            trades.Add(TradeFieldParser.Parse(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]));
        }

        return trades;
    }
}
