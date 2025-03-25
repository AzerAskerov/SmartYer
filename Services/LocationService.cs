using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;
using Microsoft.Maui.Dispatching;

namespace SmartSearch.Services
{
    public class LocationService : ILocationService
    {
        private readonly IGeolocation _geolocation;
        private bool _isTracking;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);
        private const double _distanceThresholdMeters = 10.0;
        private IDispatcherTimer? _timer;

        public event EventHandler<Location> LocationUpdated = delegate { };
        public event EventHandler<Location>? OnBusinessVisitDetected;

        public LocationService(IGeolocation geolocation)
        {
            _geolocation = geolocation;
            Debug.WriteLine("[LocationService] Initialized");
        }

        public async Task<bool> RequestLocationPermissionAsync()
        {
            try
            {
                Debug.WriteLine("[LocationService] Starting permission request process...");
                
                // First check if permissions are already granted
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                Debug.WriteLine($"[LocationService] Initial location permission status: {status}");

                if (status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
                {
                    Debug.WriteLine("[LocationService] Location permission not granted, requesting...");
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    Debug.WriteLine($"[LocationService] Location permission request result: {status}");
                    
                    // Give the system a moment to process the permission
                    await Task.Delay(1000);
                    
                    // Check status again
                    status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    Debug.WriteLine($"[LocationService] Location permission status after delay: {status}");
                }

                var isGranted = status == PermissionStatus.Granted;
                Debug.WriteLine($"[LocationService] Final permission status: {isGranted}");
                
                if (!isGranted)
                {
                    Debug.WriteLine("[LocationService] Permission not granted. Please enable in settings.");
                }
                
                return isGranted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LocationService] Error requesting location permissions: {ex}");
                return false;
            }
        }

        public async Task StartLocationUpdatesAsync()
        {
            if (_isTracking)
            {
                Debug.WriteLine("[LocationService] Location tracking is already active");
                return;
            }

            try
            {
                Debug.WriteLine("[LocationService] Starting location updates...");
                var permissionGranted = await RequestLocationPermissionAsync();
                if (!permissionGranted)
                {
                    Debug.WriteLine("[LocationService] Location permission not granted");
                    throw new Exception("Location permission not granted. Please enable location permissions in your device settings.");
                }

                _isTracking = true;
                _timer = Application.Current?.Dispatcher.CreateTimer();
                if (_timer != null)
                {
                    _timer.Interval = _updateInterval;
                    _timer.Tick += async (s, e) => await UpdateLocationAsync();
                    _timer.Start();
                    Debug.WriteLine("[LocationService] Location update timer started");

                    // Get initial location immediately
                    await UpdateLocationAsync();
                }
                else
                {
                    Debug.WriteLine("[LocationService] Failed to create timer");
                    throw new Exception("Failed to initialize location updates");
                }
            }
            catch (Exception ex)
            {
                _isTracking = false;
                Debug.WriteLine($"[LocationService] Error starting location updates: {ex}");
                throw;
            }
        }

        public async Task StopLocationUpdatesAsync()
        {
            if (!_isTracking)
            {
                Debug.WriteLine("Location tracking is not active");
                return;
            }

            try
            {
                Debug.WriteLine("Stopping location updates...");
                _timer?.Stop();
                _timer = null;
                _isTracking = false;
                Debug.WriteLine("Location updates stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping location updates: {ex}");
                throw new Exception($"Failed to stop location updates: {ex.Message}", ex);
            }
        }

        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                Debug.WriteLine("[LocationService] Getting current location...");
                var location = await _geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(5)
                });

                if (location == null)
                {
                    Debug.WriteLine("[LocationService] Unable to get location - null result");
                    throw new Exception("Unable to get location. Please check if location services are enabled.");
                }

                Debug.WriteLine($"[LocationService] Location obtained: {location.Latitude}, {location.Longitude}");
                return location;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LocationService] Error getting location: {ex}");
                throw;
            }
        }

        private async Task UpdateLocationAsync()
        {
            try
            {
                Debug.WriteLine("[LocationService] Requesting current location...");
                var location = await GetCurrentLocationAsync();
                if (location != null)
                {
                    Debug.WriteLine($"[LocationService] Location obtained: {location.Latitude}, {location.Longitude}");
                    LocationUpdated?.Invoke(this, location);
                    await CheckForBusinessVisitAsync(location);
                }
                else
                {
                    Debug.WriteLine("[LocationService] Location update returned null");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LocationService] Error updating location: {ex}");
            }
        }

        private async Task CheckForBusinessVisitAsync(Location location)
        {
            try
            {
                // TODO: In a real implementation, this would query nearby businesses
                // For now, we'll simulate a business visit detection
                if (location.Accuracy <= 20) // Only trigger if accuracy is good enough
                {
                    OnBusinessVisitDetected?.Invoke(this, location);
                    Debug.WriteLine($"Business visit detected at location: {location.Latitude}, {location.Longitude}");
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking for business visit: {ex}");
            }
        }
    }
} 