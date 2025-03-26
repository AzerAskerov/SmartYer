namespace SmartSearch.Services;

public interface IWifiService
{
    event EventHandler<double> WifiStrengthChanged;
    Task StartMonitoringAsync();
    Task StopMonitoringAsync();
} 