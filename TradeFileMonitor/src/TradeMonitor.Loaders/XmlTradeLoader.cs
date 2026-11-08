using System.Xml.Linq;
using TradeMonitor.Contracts;

namespace TradeMonitor.Loaders;

// .xml fayllarını oxuyur. Fayl bu formada olur:
//   <values>
//     <value date="2013-5-20" open="30.16" high="30.39" low="30.02" close="30.17" volume="1478200" />
//   </values>
public class XmlTradeLoader : ITradeLoader
{
    public string Name => "XML";

    public string FileExtension => ".xml";

    // XML faylını yükləyir, hər <value> elementini tapır
    // və onun atributlarını (date, open, ...) Trade obyektinə çevirir.
    public List<Trade> Load(string filePath)
    {
        var document = XDocument.Load(filePath);
        var trades = new List<Trade>();

        foreach (var element in document.Descendants("value"))
        {
            trades.Add(TradeFieldParser.Parse(
                (string)element.Attribute("date")!,
                (string)element.Attribute("open")!,
                (string)element.Attribute("high")!,
                (string)element.Attribute("low")!,
                (string)element.Attribute("close")!,
                (string)element.Attribute("volume")!));
        }

        return trades;
    }
}
