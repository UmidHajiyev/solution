using TradeMonitor.Contracts;

namespace TradeMonitor.App;

// "Loaded files" siyahısındakı bir fayl: adı, onu oxuyan loader,
// vəziyyəti ("14 rows" və ya "ERROR: ...") və fayldan oxunan sətirlər.
public class LoadedFile
{
    public string FileName { get; set; } = "";
    public string LoaderName { get; set; } = "";
    public string Status { get; set; } = "";
    public List<Trade> Trades { get; set; } = new();
}
