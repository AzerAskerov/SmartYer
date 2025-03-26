using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using SmartSearch.Models;
using SmartSearch.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;

namespace SmartSearch.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IBusinessService _businessService;
        private readonly ILocationService _locationService;
        private readonly ILogger<MainViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<Business> _localBusinesses = new();

        [ObservableProperty]
        private ObservableCollection<Business> _googlePlaces = new();

        [ObservableProperty]
        private ObservableCollection<Business> _nearbyBusinesses = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private Location? _currentLocation;

        [ObservableProperty]
        private string _selectedCategory = string.Empty;

        [ObservableProperty]
        private List<string> _suggestedCategories = new();

        [ObservableProperty]
        private ObservableCollection<string> _popularCategories = new();

        private Location? _lastKnownLocation;

        public MainViewModel(
            IBusinessService businessService,
            ILocationService locationService,
            ILogger<MainViewModel> logger)
        {
            _businessService = businessService;
            _locationService = locationService;
            _logger = logger;

            _locationService.LocationChanged += OnLocationChanged;
            _locationService.WifiStrengthChanged += OnWifiStrengthChanged;
        }

        [RelayCommand]
        private async Task NotificationsAsync()
        {
            await Shell.Current.GoToAsync("//NotificationsPage");
        }

        [RelayCommand]
        private async Task MenuAsync()
        {
            await Shell.Current.GoToAsync("//MenuPage");
        }

        [RelayCommand]
        private async Task SeeAllRecentAsync()
        {
            await Shell.Current.GoToAsync("//RecentBusinessesPage");
        }

        [RelayCommand]
        private async Task SeeAllNearbyAsync()
        {
            await Shell.Current.GoToAsync("//NearbyBusinessesPage");
        }

        [RelayCommand]
        private async Task HomeAsync()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        private async Task ExploreAsync()
        {
            await Shell.Current.GoToAsync("//ExplorePage");
        }

        [RelayCommand]
        private async Task ProfileAsync()
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            try
            {
                IsRefreshing = true;
                await LoadNearbyBusinessesAsync();
                await LoadRecentlyVisitedAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async void OnLocationChanged(object? sender, Location location)
        {
            try
            {
                _lastKnownLocation = location;
                await SearchNearbyBusinessesAsync(location.Latitude, location.Longitude);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling location change");
                ErrorMessage = "Failed to update nearby businesses.";
            }
        }

        private async void OnWifiStrengthChanged(object? sender, double strength)
        {
            try
            {
                if (_lastKnownLocation != null)
                {
                    await SearchNearbyBusinessesAsync(_lastKnownLocation.Latitude, _lastKnownLocation.Longitude);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling WiFi strength change");
                ErrorMessage = "Failed to update nearby businesses.";
            }
        }

        private async Task SearchNearbyBusinessesAsync(double latitude, double longitude)
        {
            try
            {
                Debug.WriteLine($"[MainViewModel] Loading nearby businesses for location: {latitude}, {longitude}");
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                var businesses = await _businessService.SearchNearbyBusinessesAsync(latitude, longitude);
                Debug.WriteLine($"[MainViewModel] Found {businesses.Count} nearby businesses");

                // Separate businesses into local and Google Places
                var localBusinesses = businesses.Where(b => !b.IsFromGooglePlaces).Take(3).ToList();
                var googlePlaces = businesses.Where(b => b.IsFromGooglePlaces).Take(3).ToList();

                // Update collections on the main thread
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LocalBusinesses.Clear();
                    foreach (var business in localBusinesses)
                    {
                        LocalBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added local business: {business.Name}");
                    }

                    GooglePlaces.Clear();
                    foreach (var business in googlePlaces)
                    {
                        GooglePlaces.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added Google Place: {business.Name}");
                    }

                    // Update NearbyBusinesses with 3 local and 3 Google Places businesses
                    NearbyBusinesses.Clear();
                    foreach (var business in localBusinesses)
                    {
                        NearbyBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added nearby local business: {business.Name}");
                    }
                    foreach (var business in googlePlaces)
                    {
                        NearbyBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added nearby Google Place: {business.Name}");
                    }

                    Debug.WriteLine($"[MainViewModel] Updated UI with {LocalBusinesses.Count} local businesses, {GooglePlaces.Count} Google Places, and {NearbyBusinesses.Count} total nearby businesses");
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching nearby businesses");
                ErrorMessage = "Failed to find nearby businesses.";
                HasError = true;
                Debug.WriteLine($"[MainViewModel] Error loading nearby businesses: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadNearbyBusinessesAsync()
        {
            if (CurrentLocation == null)
            {
                Debug.WriteLine("[MainViewModel] Cannot load nearby businesses: CurrentLocation is null");
                return;
            }

            await SearchNearbyBusinessesAsync(CurrentLocation.Latitude, CurrentLocation.Longitude);
        }

        private async Task LoadSampleDataAsync()
        {
            try
            {
                Debug.WriteLine("[MainViewModel] Loading sample data due to location error...");
                IsLoading = true;
                
                var businesses = await _businessService.SearchNearbyBusinessesAsync(0, 0);
                
                // Separate businesses into local and Google Places
                var localBusinesses = businesses.Where(b => !b.IsFromGooglePlaces).Take(3).ToList();
                var googlePlaces = businesses.Where(b => b.IsFromGooglePlaces).Take(3).ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LocalBusinesses.Clear();
                    foreach (var business in localBusinesses)
                    {
                        LocalBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added local business: {business.Name}");
                    }

                    GooglePlaces.Clear();
                    foreach (var business in googlePlaces)
                    {
                        GooglePlaces.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added Google Place: {business.Name}");
                    }

                    // Update NearbyBusinesses with 3 local and 3 Google Places businesses
                    NearbyBusinesses.Clear();
                    foreach (var business in localBusinesses)
                    {
                        NearbyBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added nearby local business: {business.Name}");
                    }
                    foreach (var business in googlePlaces)
                    {
                        NearbyBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added nearby Google Place: {business.Name}");
                    }

                    Debug.WriteLine($"[MainViewModel] Updated UI with {LocalBusinesses.Count} local businesses, {GooglePlaces.Count} Google Places, and {NearbyBusinesses.Count} total nearby businesses");
                });
                
                await LoadRecentlyVisitedAsync();
                Debug.WriteLine("[MainViewModel] Sample data loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel] Error loading sample data: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // Check and request permissions
                var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (locationStatus != PermissionStatus.Granted)
                {
                    locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (locationStatus != PermissionStatus.Granted)
                    {
                        _logger.LogWarning("Location permission not granted");
                        await LoadSampleDataAsync();
                        return;
                    }
                }

                // Start location updates
                await _locationService.StartLocationUpdatesAsync();
                await _locationService.StartWifiUpdatesAsync();

                // Load initial data
                await LoadNearbyBusinessesAsync();
                await LoadRecentlyVisitedAsync();
                await LoadPopularCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during initialization");
                ErrorMessage = "Failed to initialize app. Please try again.";
                HasError = true;
                await LoadSampleDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadRecentlyVisitedAsync()
        {
            try
            {
                var businesses = await _businessService.GetRecentBusinessesAsync();
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LocalBusinesses.Clear();
                    foreach (var business in businesses)
                    {
                        LocalBusinesses.Add(business);
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                Debug.WriteLine($"Error loading recently visited: {ex}");
            }
        }

        private async Task LoadPopularCategoriesAsync()
        {
            try
            {
                var categories = await _businessService.GetPopularCategoriesAsync();
                PopularCategories.Clear();
                foreach (var category in categories)
                {
                    PopularCategories.Add(category);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading popular categories");
            }
        }

        public void Cleanup()
        {
            _locationService.LocationChanged -= OnLocationChanged;
            _locationService.WifiStrengthChanged -= OnWifiStrengthChanged;
            _locationService.StopLocationUpdatesAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task NavigateToBusinessDetailsAsync(Business business)
        {
            if (business == null) return;
            var parameters = new Dictionary<string, object>
            {
                { "Business", business }
            };
            await Shell.Current.GoToAsync("BusinessDetailsPage", parameters);
        }

        partial void OnSearchTextChanged(string? value)
        {
            // No-op for now
        }

        [RelayCommand]
        private async Task StartLocationTrackingAsync()
        {
            try
            {
                await _locationService.StartLocationUpdatesAsync();
                await _locationService.StartWifiUpdatesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting location tracking");
                ErrorMessage = "Failed to start location tracking.";
                HasError = true;
            }
        }

        [RelayCommand]
        private async Task StopLocationTrackingAsync()
        {
            try
            {
                await _locationService.StopLocationUpdatesAsync();
                await _locationService.StopWifiUpdatesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping location tracking");
                ErrorMessage = "Failed to stop location tracking.";
                HasError = true;
            }
        }
    }
} 