using System.Globalization;
using TradeMonitor.Contracts;

namespace TradeMonitor.Loaders;

// Altı mətn dəyərini bir Trade obyektinə çevirir. Hər üç loader bunu istifadə edir.
public static class TradeFieldParser
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    // Mətnləri tarix və rəqəmlərə çevirir.
    // InvariantCulture: kompüterin regional ayarından asılı olmayaraq onluq ayırıcı həmişə "." olur.
    // "yyyy-M-d" formatı "2013-5-1" kimi bir rəqəmli ay/günü də qəbul edir.
    public static Trade Parse(string date, string open, string high, string low, string close, string volume)
    {
        return new Trade
        {
            Date = DateTime.ParseExact(date.Trim(), "yyyy-M-d", Culture),
            Open = decimal.Parse(open, Culture),
            High = decimal.Parse(high, Culture),
            Low = decimal.Parse(low, Culture),
            Close = decimal.Parse(close, Culture),
            Volume = long.Parse(volume, Culture),
        };
    }
}
