using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using TradeMonitor.Contracts;

namespace TradeMonitor.App;

// Əsas pəncərənin kodu: ayarları oxuyur, plugin-ləri tapır, qovluğu yoxlayır və faylları yükləyir.
//   _loaders      - fayl uzantısı -> loader (".csv" -> CsvTradeLoader)
//   _knownFiles   - artıq yükləməyə başladığımız fayllar, hər fayl yalnız bir dəfə yüklənsin
//   _loadedFiles  - soldakı siyahı; ObservableCollection olduğu üçün ekran avtomatik yenilənir
//   _timer        - hər N saniyədən bir qovluğu yoxlayan taymer (UI thread-də işləyir)
//   _filesLoading - hal-hazırda neçə fayl yüklənir
public partial class MainWindow : Window
{
    private AppSettings _settings = new();
    private Dictionary<string, ITradeLoader> _loaders = new();
    private readonly HashSet<string> _knownFiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ObservableCollection<LoadedFile> _loadedFiles = new();
    private readonly DispatcherTimer _timer = new();
    private int _filesLoading;

    // Pəncərənin elementlərini yaradır və soldakı siyahını _loadedFiles kolleksiyasına bağlayır.
    public MainWindow()
    {
        InitializeComponent();
        FilesList.ItemsSource = _loadedFiles;
    }

    // Pəncərə açılanda bir dəfə işləyir:
    // ayarları oxuyur (alınmasa xəta göstərib bağlanır), plugin-ləri yükləyir, Input qovluğunu yaradır,
    // yuxarıdakı məlumat sətrini doldurur, taymeri işə salır və ilk yoxlamanı dərhal edir.
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _settings = AppSettings.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not read appsettings.json: " + ex.Message, "Error");
            Close();
            return;
        }

        _loaders = PluginLoader.LoadLoaders(_settings.PluginFolder, _settings.EnabledLoaders);
        Directory.CreateDirectory(_settings.InputFolder);

        InfoText.Text =
            $"Watching folder: {_settings.InputFolder}\n" +
            $"Checking every {_settings.CheckIntervalSeconds} seconds. " +
            $"Loaders: {string.Join(", ", _loaders.Values.Select(l => $"{l.Name} ({l.FileExtension})"))}";

        if (_loaders.Count == 0)
        {
            MessageBox.Show("No loaders found. Check the Plugins folder and EnabledLoaders in appsettings.json.", "Warning");
        }

        _timer.Interval = TimeSpan.FromSeconds(_settings.CheckIntervalSeconds);
        _timer.Tick += (s, args) => CheckForNewFiles();
        _timer.Start();

        CheckForNewFiles();
        UpdateStatus();
    }

    // Qovluqda yeni faylları axtarır. Artıq görülən və loader-i olmayan faylları keçir.
    // Yeni faylı "bilinən" kimi qeyd edib yükləməyə başlayır.
    // Burada "await" YOXDUR: yükləməni başladıb dərhal növbəti fayla keçir,
    // ona görə bütün yeni fayllar EYNİ VAXTDA (paralel) yüklənir.
    private void CheckForNewFiles()
    {
        foreach (string path in Directory.GetFiles(_settings.InputFolder))
        {
            if (_knownFiles.Contains(path))
            {
                continue;
            }

            if (!_loaders.TryGetValue(Path.GetExtension(path), out ITradeLoader? loader))
            {
                continue;
            }

            _knownFiles.Add(path);
            _ = LoadFileAsync(path, loader);
        }
    }

    // Bir faylı yükləyir və siyahıya əlavə edir.
    // Task.Run faylı arxa planda oxuyur, ona görə pəncərə donmur; "await" UI-ni bloklamadan gözləyir.
    // IOException: fayl hələ kopyalanır - onu unuduruq, növbəti yoxlamada yenidən cəhd edilir.
    // Başqa xəta: fayl səhvdir - proqram çökmür, siyahıda "ERROR: ..." göstərilir.
    // Hər fayl hazır olan kimi göstərilir, digərlərini gözləmir.
    private async Task LoadFileAsync(string path, ITradeLoader loader)
    {
        _filesLoading++;
        UpdateStatus();

        var file = new LoadedFile
        {
            FileName = Path.GetFileName(path),
            LoaderName = loader.Name,
        };

        try
        {
            file.Trades = await Task.Run(() => loader.Load(path));
            file.Status = $"{file.Trades.Count} rows";
        }
        catch (IOException)
        {
            _knownFiles.Remove(path);
            _filesLoading--;
            UpdateStatus();
            return;
        }
        catch (Exception ex)
        {
            file.Status = "ERROR: " + ex.Message;
        }

        _loadedFiles.Add(file);
        if (FilesList.SelectedItem == null)
        {
            FilesList.SelectedItem = file;
        }

        _filesLoading--;
        UpdateStatus();
    }

    // Aşağıdakı vəziyyət sətrini yeniləyir: neçə fayl yüklənib və neçəsi indi yüklənir.
    private void UpdateStatus()
    {
        StatusText.Text = $"Files loaded: {_loadedFiles.Count}   |   Files loading now: {_filesLoading}";
    }
}
