namespace TradeMonitor.Contracts;

// Bir ticarət sətri (bir günün məlumatı): tarix, açılış, ən yüksək, ən aşağı, bağlanış qiyməti və həcm.
// Bütün loader-lər (CSV, TXT, XML) faylı oxuyub bu class-ın obyektlərini qaytarır.
public class Trade
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}
