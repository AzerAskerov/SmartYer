using Microsoft.Maui.Devices.Sensors;
using SmartSearch.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace SmartSearch.Services;

public class BusinessService : IBusinessService
{
    private readonly HttpClient _httpClient;
    private readonly IGooglePlacesService _googlePlacesService;
    private readonly string _apiKey;
    private const string _baseUrl = "https://api.example.com/v1"; // Replace with actual API URL
    private readonly ILogger<BusinessService> _logger;
    private readonly List<Business> _sampleBusinesses;

    private readonly List<Business> _recentlyVisited = new()
    {
        new Business
        {
            Id = "1",
            Name = "Ali's Repair",
            Rating = 4.5,
            ReviewCount = 128
        },
        new Business
        {
            Id = "2",
            Name = "Car Service",
            Rating = 4.0,
            ReviewCount = 256
        },
        new Business
        {
            Id = "3",
            Name = "Pizza Corner",
            Rating = 4.2,
            ReviewCount = 89
        }
    };

    public BusinessService(
        HttpClient httpClient,
        IGooglePlacesService googlePlacesService,
        ILogger<BusinessService> logger)
    {
        _httpClient = httpClient;
        _googlePlacesService = googlePlacesService;
        _logger = logger;
        _apiKey = "test_api_key"; // TODO: Replace with actual API key
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _sampleBusinesses = InitializeSampleData();
    }

    public async Task<List<Business>> GetRecentBusinessesAsync()
    {
        try
        {
            _logger.LogInformation("Getting recent businesses");
            await Task.Delay(200); // Simulate network delay
            return _sampleBusinesses.OrderByDescending(b => b.LastUpdated).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent businesses");
            throw;
        }
    }

    public async Task<List<Business>> GetLocalBusinessesAsync(double latitude, double longitude)
    {
        try
        {
            _logger.LogInformation("Getting local businesses");
            await Task.Delay(200); // Simulate network delay
            var businesses = _sampleBusinesses.Select(b => new Business
            {
                Id = b.Id,
                Name = b.Name,
                Category = b.Category,
                Rating = b.Rating,
                ReviewCount = b.ReviewCount,
                Distance = b.Distance,
                Features = b.Features,
                Location = b.Location,
                LastUpdated = b.LastUpdated,
                Photos = b.Photos,
                IsFromGooglePlaces = false
            }).ToList();
            return businesses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting local businesses");
            throw;
        }
    }

    public async Task<List<Business>> SearchNearbyBusinessesAsync(double latitude, double longitude)
    {
        try
        {
            _logger.LogInformation("Searching for nearby businesses");
            
            // Get businesses from local database
            var localBusinesses = await GetLocalBusinessesAsync(latitude, longitude);
            foreach (var business in localBusinesses)
            {
                business.IsFromGooglePlaces = false;
            }

            // Get businesses from Google Places API
            var googlePlaces = await _googlePlacesService.SearchNearbyPlacesAsync(latitude, longitude, 5000);
            foreach (var business in googlePlaces)
            {
                business.IsFromGooglePlaces = true;
            }

            // Combine and sort results
            var allBusinesses = localBusinesses.Concat(googlePlaces)
                .OrderByDescending(b => b.Rating)
                .ThenBy(b => CalculateDistance(latitude, longitude, b.Location.Latitude, b.Location.Longitude))
                .ToList();

            _logger.LogInformation("Found {Count} nearby businesses", allBusinesses.Count);
            return allBusinesses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching nearby businesses");
            throw;
        }
    }

    private List<Business> InitializeSampleData()
    {
        return new List<Business>
                {
                    new Business
                    {
                        Id = "1",
                Name = "Ali's Laptop Repair",
                Category = "Computer Repair",
                        Rating = 4.5,
                ReviewCount = 128,
                Distance = 0.5,
                Features = new List<string> { "technical knowledge", "fair price" },
                Location = new BusinessLocation 
                { 
                    Latitude = 40.4093,
                    Longitude = 49.8671,
                    Address = "Baku, Azerbaijan"
                },
                LastUpdated = DateTime.Now,
                Photos = new List<string> { "sample_repair.jpg" }
            },
            new Business
            {
                Id = "2",
                Name = "Ehmed Avto Service",
                Category = "Auto Repair Zone",
                Rating = 4.2,
                ReviewCount = 256,
                Distance = 1.2,
                Features = new List<string> { "quality repairs", "multiple services" },
                Location = new BusinessLocation 
                { 
                    Latitude = 40.4093,
                    Longitude = 49.8671,
                    Address = "Baku, Azerbaijan"
                },
                LastUpdated = DateTime.Now.AddHours(-1),
                Photos = new List<string> { "sample_auto.jpg" }
            },
            new Business
            {
                Id = "3",
                Name = "Aygun Tyre Service",
                Category = "Tire Shop",
                Rating = 4.0,
                ReviewCount = 89,
                Distance = 0.8,
                Features = new List<string> { "good prices", "quality tires" },
                Location = new BusinessLocation 
                { 
                    Latitude = 40.4093,
                    Longitude = 49.8671,
                    Address = "Baku, Azerbaijan"
                },
                LastUpdated = DateTime.Now.AddHours(-2),
                Photos = new List<string> { "sample_tire.jpg" }
            }
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRad(double degrees) => degrees * Math.PI / 180;

    public async Task<List<string>> GetPopularCategoriesAsync()
    {
        try
        {
            await Task.Delay(200);
            return new List<string>
            {
                "Restaurants",
                "Auto Service",
                "Computer Repair",
                "Beauty & Spa",
                "Shopping"
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting popular categories: {ex}");
            throw new Exception("Failed to get popular categories", ex);
        }
    }

    public async Task<List<Business>> GetRecentlyVisitedAsync()
    {
        try
        {
            Debug.WriteLine("[BusinessService] Getting recently visited businesses");
            await Task.Delay(200); // Add a small delay to simulate network call
            
            Debug.WriteLine($"[BusinessService] Returning {_recentlyVisited.Count} recently visited businesses:");
            foreach (var business in _recentlyVisited)
            {
                Debug.WriteLine($"[BusinessService] - {business.Name} ({business.Rating})");
            }
            
            return _recentlyVisited;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[BusinessService] Error getting recently visited businesses: {ex}");
            throw;
        }
    }

    public async Task<List<string>> GetSuggestedCategoriesAsync(string query)
    {
        try
        {
            await Task.Delay(200);
            return new List<string>
            {
                "Auto Repair",
                "Computer Service",
                "Mobile Repair",
                "Electronics"
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting suggested categories: {ex}");
            throw new Exception("Failed to get suggested categories", ex);
        }
    }

    public async Task<Business> GetBusinessDetailsAsync(string businessId)
    {
        try
        {
            // For testing, return mock data
            return new Business
            {
                Id = businessId,
                Name = "Test Business",
                Description = "A test business",
                Address = "123 Test St",
                Rating = 4.5,
                ReviewCount = 10,
                Category = "Restaurant",
                Tags = new List<string> { "Bar" }
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get business details", ex);
        }
    }

    public async Task<List<Review>> GetBusinessReviewsAsync(string businessId, int page = 1, int pageSize = 20)
    {
        try
        {
            // For testing, return mock data
            return new List<Review>
            {
                new Review
                {
                    Id = "1",
                    BusinessId = businessId,
                    UserId = "user1",
                    Rating = 4,
                    Content = "Great place!",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get business reviews", ex);
        }
    }

    public async Task<Review> CreateReviewAsync(string businessId, Review review)
    {
        try
        {
            // Simulate API call delay
            await Task.Delay(300);
            
            // For testing, return the same review with an ID
            review.Id = Guid.NewGuid().ToString();
            review.CreatedAt = DateTime.UtcNow;
            return review;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create review", ex);
        }
    }

    public async Task<Review> UpdateReviewAsync(string businessId, string reviewId, Review review)
    {
        try
        {
            // Simulate API call delay
            await Task.Delay(300);
            
            // For testing, return the updated review
            review.Id = reviewId;
            review.UpdatedAt = DateTime.UtcNow;
            return review;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update review", ex);
        }
    }

    public async Task DeleteReviewAsync(string businessId, string reviewId)
    {
        try
        {
            // Simulate API call delay
            await Task.Delay(300);
            Debug.WriteLine($"Review {reviewId} for business {businessId} deleted");
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete review", ex);
        }
    }

    public async Task MarkReviewHelpfulAsync(string businessId, string reviewId)
    {
        try
        {
            // Simulate API call delay
            await Task.Delay(300);
            Debug.WriteLine($"Review {reviewId} for business {businessId} marked as helpful");
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to mark review as helpful", ex);
        }
    }
} 