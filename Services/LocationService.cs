using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;
using Microsoft.Maui.Dispatching;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SmartSearch.Services
{
    public class LocationService : ILocationService
    {
        private readonly IGeolocation _geolocation;
        private readonly IWifiService _wifiService;
        private readonly IBusinessService _businessService;
        private readonly ILogger<LocationService> _logger;
        private readonly SemaphoreSlim _locationUpdateLock = new(1, 1);
        private readonly SemaphoreSlim _wifiUpdateLock = new(1, 1);
        private Location? _lastKnownLocation;
        private double _lastKnownWifiStrength;
        private const double LOCATION_CHANGE_THRESHOLD_METERS = 10; // 10 meters threshold
        private const double WIFI_STRENGTH_CHANGE_THRESHOLD_DBM = 5; // 5 dBm threshold
        private const int MIN_UPDATE_INTERVAL_MS = 1000; // Minimum 1 second between updates
        private DateTime _lastUpdateTime = DateTime.MinValue;

        public event EventHandler<Location>? LocationChanged;
        public event EventHandler<double>? WifiStrengthChanged;

        public LocationService(
            IGeolocation geolocation,
            IWifiService wifiService,
            IBusinessService businessService,
            ILogger<LocationService> logger)
        {
            _geolocation = geolocation;
            _wifiService = wifiService;
            _businessService = businessService;
            _logger = logger;
        }

        public async Task StartLocationUpdatesAsync()
        {
            try
            {
                _logger.LogInformation("Starting location updates");

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        _logger.LogWarning("Location permission not granted");
                        return;
                    }
                }

                try
                {
                    // Get initial location
                    var location = await _geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(5)
                    });

                    if (location != null)
                    {
                        _lastKnownLocation = location;
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            try
                            {
                                LocationChanged?.Invoke(this, location);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error invoking initial LocationChanged event");
                            }
                        });
                    }

                    var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));
                    _geolocation.LocationChanged += OnLocationChanged;
        
                    // Start listening in the background
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        try
                        {
                            await _geolocation.StartListeningForegroundAsync(request);
                            _logger.LogInformation("Location updates started successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error starting location updates");
                            _geolocation.LocationChanged -= OnLocationChanged;
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing location updates");
                    _geolocation.LocationChanged -= OnLocationChanged;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting location updates");
            }
        }

        public async Task StopLocationUpdatesAsync()
        {
            try
            {
                _logger.LogInformation("Stopping location updates");
                _geolocation.LocationChanged -= OnLocationChanged;
                _logger.LogInformation("Location updates stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping location updates");
                throw;
            }
        }

        private async void OnLocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            try
            {
                if (!await _locationUpdateLock.WaitAsync(0))
                {
                    _logger.LogDebug("Location update already in progress, skipping");
                    return;
                }

                try
                {
                    var now = DateTime.UtcNow;
                    if ((now - _lastUpdateTime).TotalMilliseconds < MIN_UPDATE_INTERVAL_MS)
                    {
                        _logger.LogDebug("Update interval too short, skipping");
                        return;
                    }

                    var location = e.Location;
                    if (location == null)
                    {
                        _logger.LogWarning("Received null location update");
                        return;
                    }

                    _logger.LogDebug("Location changed: {Latitude}, {Longitude}", location.Latitude, location.Longitude);

                    if (_lastKnownLocation != null)
                    {
                        var distance = Location.CalculateDistance(
                            _lastKnownLocation.Latitude,
                            _lastKnownLocation.Longitude,
                            location.Latitude,
                            location.Longitude,
                            DistanceUnits.Kilometers);

                        _logger.LogDebug("Distance moved: {Distance} meters", distance * 1000);

                        // Only trigger if moved more than threshold
                        if (distance * 1000 < LOCATION_CHANGE_THRESHOLD_METERS)
                        {
                            _logger.LogDebug("Location change below threshold, skipping update");
                            return;
                        }
                    }

                    _lastKnownLocation = location;
                    _lastUpdateTime = now;
            
                    // Invoke location changed event on the main thread
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            LocationChanged?.Invoke(this, location);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error invoking LocationChanged event");
                        }
                    });

                    // Update nearby businesses
                    try
                    {
                        await _businessService.SearchNearbyBusinessesAsync(location.Latitude, location.Longitude);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating nearby businesses");
                    }
                }
                finally
                {
                    _locationUpdateLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling location change");
            }
        }

        public async Task StartWifiUpdatesAsync()
        {
            try
            {
                _logger.LogInformation("Starting WiFi updates");
                _wifiService.WifiStrengthChanged += OnWifiStrengthChanged;
                await _wifiService.StartMonitoringAsync();
                _logger.LogInformation("WiFi updates started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting WiFi updates");
                throw;
            }
        }

        public async Task StopWifiUpdatesAsync()
        {
            try
            {
                _logger.LogInformation("Stopping WiFi updates");
                _wifiService.WifiStrengthChanged -= OnWifiStrengthChanged;
                await _wifiService.StopMonitoringAsync();
                _logger.LogInformation("WiFi updates stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping WiFi updates");
                throw;
            }
        }

        private async void OnWifiStrengthChanged(object? sender, double strength)
        {
            try
            {
                if (!await _wifiUpdateLock.WaitAsync(0))
                {
                    _logger.LogDebug("WiFi update already in progress, skipping");
                    return;
                }

                try
                {
                    _logger.LogDebug("WiFi strength changed: {Strength} dBm", strength);

                    if (_lastKnownWifiStrength != 0)
                    {
                        var strengthDiff = Math.Abs(strength - _lastKnownWifiStrength);
                        _logger.LogDebug("WiFi strength change: {StrengthDiff} dBm", strengthDiff);

                        // Only trigger if strength changed more than threshold
                        if (strengthDiff < WIFI_STRENGTH_CHANGE_THRESHOLD_DBM)
                        {
                            _logger.LogDebug("WiFi strength change below threshold, skipping update");
                            return;
                        }
                    }

                    _lastKnownWifiStrength = strength;
                    WifiStrengthChanged?.Invoke(this, strength);

                    if (_lastKnownLocation != null)
                    {
                        await _businessService.SearchNearbyBusinessesAsync(_lastKnownLocation.Latitude, _lastKnownLocation.Longitude);
                    }
                }
                finally
                {
                    _wifiUpdateLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling WiFi strength change");
            }
        }
    }
} 