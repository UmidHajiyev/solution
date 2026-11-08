# Trade File Monitor

WPF app that watches a folder and shows trades from new CSV, TXT and XML files.

## Run

Open `TradeFileMonitor.sln`, set `TradeMonitor.App` as startup project and run.
Then copy files from `SampleData` into the `Input` folder shown in the window.

## Settings

`appsettings.json` next to the exe:

```json
{
  "InputFolder": "Input",
  "CheckIntervalSeconds": 5,
  "PluginFolder": "Plugins",
  "EnabledLoaders": [ "CSV", "TXT", "XML" ]
}
```

## New loader

Make a class library that references `TradeMonitor.Contracts`, implement `ITradeLoader`,
put the dll into `Plugins` and add its name to `EnabledLoaders`.
