using TradeMonitor.Contracts;

namespace TradeMonitor.Loaders;

// .txt fayllarını oxuyur: dəyərlər nöqtəli vergüllə (;) ayrılıb, birinci sətir başlıqdır.
// Nümunə:
//   Date;Open;High;Low;Close;Volume
//   2013-5-20;30.16;30.39;30.02;30.17;1478200
public class TxtTradeLoader : ITradeLoader
{
    public string Name => "TXT";

    public string FileExtension => ".txt";

    // Başlıq sətrini atlayır (Skip(1)), qalan sətirləri ";" ilə 6 hissəyə bölüb Trade-ə çevirir.
    // Boş sətirlər keçilir, 6 dəyər olmayan sətir xəta verir.
    public List<Trade> Load(string filePath)
    {
        var trades = new List<Trade>();

        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(';');
            if (parts.Length != 6)
            {
                throw new FormatException($"Line \"{line}\" should have 6 values separated by semicolons.");
            }

            trades.Add(TradeFieldParser.Parse(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]));
        }

        return trades;
    }
}
