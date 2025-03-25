using Microsoft.Maui.Devices.Sensors;

namespace SmartSearch.Services;

public interface ILocationManager
{
    bool IsLocationEnabled();
    Task<bool> RequestLocationPermissionAsync();
} 