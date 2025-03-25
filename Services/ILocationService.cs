using Microsoft.Maui.Devices.Sensors;

namespace SmartSearch.Services
{
    public interface ILocationService
    {
        event EventHandler<Location> LocationUpdated;
        Task<Location> GetCurrentLocationAsync();
        Task StartLocationUpdatesAsync();
        Task StopLocationUpdatesAsync();
    }
} 