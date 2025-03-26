using Microsoft.Maui.Devices.Sensors;

namespace SmartSearch.Services;

public interface ILocationService
{
    event EventHandler<Location> LocationChanged;
    event EventHandler<double> WifiStrengthChanged;
    Task StartLocationUpdatesAsync();
    Task StopLocationUpdatesAsync();
    Task StartWifiUpdatesAsync();
    Task StopWifiUpdatesAsync();
} 