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

namespace SmartSearch.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IBusinessService _businessService;
        private readonly ILocationService _locationService;

        [ObservableProperty]
        private ObservableCollection<Business> _nearbyBusinesses = new();

        [ObservableProperty]
        private ObservableCollection<Business> _recentlyVisited = new();

        [ObservableProperty]
        private ObservableCollection<string> _popularCategories = new();

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

        public MainViewModel(IBusinessService businessService, ILocationService locationService)
        {
            _businessService = businessService;
            _locationService = locationService;
            _locationService.LocationUpdated += OnLocationUpdated;
            Debug.WriteLine("[MainViewModel] Constructor - Initialized and subscribed to location updates");
            
            // Load initial data
            Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine("[MainViewModel] Starting initialization...");
                    await InitializeAsync();
                    Debug.WriteLine("[MainViewModel] Initialization completed");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[MainViewModel] Error during initialization: {ex}");
                    ErrorMessage = ex.Message;
                    HasError = true;
                    
                    // Even if location fails, try to load some sample data
                    await LoadSampleDataAsync();
                }
            });
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

        private async void OnLocationUpdated(object? sender, Location location)
        {
            Debug.WriteLine($"Location updated: {location.Latitude}, {location.Longitude}");
            CurrentLocation = location;
            await LoadNearbyBusinessesAsync();
        }

        private async Task LoadNearbyBusinessesAsync()
        {
            if (CurrentLocation == null)
            {
                Debug.WriteLine("[MainViewModel] Cannot load nearby businesses: CurrentLocation is null");
                return;
            }

            try
            {
                Debug.WriteLine($"[MainViewModel] Loading nearby businesses for location: {CurrentLocation.Latitude}, {CurrentLocation.Longitude}");
                IsLoading = true;
                HasError = false;

                var result = await _businessService.SearchNearbyBusinessesAsync(
                    CurrentLocation.Latitude,
                    CurrentLocation.Longitude,
                    radius: 5000); // 5km radius

                Debug.WriteLine($"[MainViewModel] Found {result.Businesses.Count} nearby businesses");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    NearbyBusinesses.Clear();
                    foreach (var business in result.Businesses)
                    {
                        NearbyBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added business: {business.Name}");
                    }
                    Debug.WriteLine($"[MainViewModel] Updated UI with {NearbyBusinesses.Count} businesses");
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                Debug.WriteLine($"[MainViewModel] Error loading nearby businesses: {ex}");
            }
            finally
            {
                IsLoading = false;
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
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        private async Task LoadSampleDataAsync()
        {
            try
            {
                Debug.WriteLine("[MainViewModel] Loading sample data due to location error...");
                IsLoading = true;
                
                var result = await _businessService.SearchNearbyBusinessesAsync(0, 0, 5000);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    NearbyBusinesses.Clear();
                    foreach (var business in result.Businesses)
                    {
                        NearbyBusinesses.Add(business);
                        Debug.WriteLine($"[MainViewModel] Added sample business: {business.Name}");
                    }
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
                Debug.WriteLine("[MainViewModel] Starting InitializeAsync...");
                IsLoading = true;
                HasError = false;

                // First load recently visited businesses
                Debug.WriteLine("[MainViewModel] Loading recently visited businesses...");
                await LoadRecentlyVisitedAsync();
                Debug.WriteLine("[MainViewModel] Recently visited businesses loaded");

                // Then try to start location updates
                Debug.WriteLine("[MainViewModel] Starting location updates...");
                await _locationService.StartLocationUpdatesAsync();
                Debug.WriteLine("[MainViewModel] Location updates started successfully");

                Debug.WriteLine("[MainViewModel] InitializeAsync completed successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error initializing app: {ex.Message}";
                HasError = true;
                Debug.WriteLine($"[MainViewModel] Error in InitializeAsync: {ex}");
                throw; // Rethrow to trigger the sample data loading
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
                var businesses = await _businessService.GetRecentlyVisitedAsync();
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    RecentlyVisited.Clear();
                    foreach (var business in businesses)
                    {
                        RecentlyVisited.Add(business);
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

        public void Cleanup()
        {
            _locationService.LocationUpdated -= OnLocationUpdated;
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

        private async Task LoadSuggestedCategoriesAsync(string query)
        {
            try
            {
                var suggestions = await _businessService.GetSuggestedCategoriesAsync(query);
                SuggestedCategories = new List<string>(suggestions);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        partial void OnSearchTextChanged(string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                LoadSuggestedCategoriesAsync(value).ConfigureAwait(false);
            }
            else
            {
                SuggestedCategories.Clear();
            }
        }

        [RelayCommand]
        private async Task StartLocationTrackingAsync()
        {
            try
            {
                await _locationService.StartLocationUpdatesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error starting location tracking: {ex.Message}";
                HasError = true;
                Debug.WriteLine($"Error in StartLocationTrackingAsync: {ex}");
            }
        }

        [RelayCommand]
        private async Task StopLocationTrackingAsync()
        {
            try
            {
                await _locationService.StopLocationUpdatesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error stopping location tracking: {ex.Message}";
                HasError = true;
                Debug.WriteLine($"Error in StopLocationTrackingAsync: {ex}");
            }
        }
    }
} 