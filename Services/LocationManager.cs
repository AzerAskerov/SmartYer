using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace SmartSearch.Services;

public class LocationManager : ILocationManager
{
    public static LocationManager Default { get; } = new();

    public bool IsLocationEnabled()
    {
#if ANDROID
        var locationManager = Android.App.Application.Context.GetSystemService(Android.Content.Context.LocationService) as Android.Locations.LocationManager;
        return locationManager?.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider) == true;
#else
        return true;
#endif
    }

    public async Task<bool> RequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        return status == PermissionStatus.Granted;
    }
} 