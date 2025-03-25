using Microsoft.Maui.Devices.Sensors;
using SmartSearch.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SmartSearch.Services;

public class BusinessService : IBusinessService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string _baseUrl = "https://api.example.com/v1"; // Replace with actual API URL

    private readonly List<Business> _sampleBusinesses = new()
    {
        new Business
        {
            Id = "1",
            Name = "Ali's Laptop Repair",
            Category = "Computer Repair",
            Rating = 4.5,
            ReviewCount = 128,
            Distance = 0.5,
            Features = new List<string> { "technical knowledge", "fair price" }
        },
        new Business
        {
            Id = "2",
            Name = "Ehmed Avto Service",
            Category = "Auto Repair Zone",
            Rating = 4.2,
            ReviewCount = 256,
            Distance = 1.2,
            Features = new List<string> { "quality repairs", "multiple services" }
        },
        new Business
        {
            Id = "3",
            Name = "Aygun Tyre Service",
            Category = "Tire Shop",
            Rating = 4.0,
            ReviewCount = 89,
            Distance = 0.8,
            Features = new List<string> { "good prices", "quality tires" }
        }
    };

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

    public BusinessService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = "test_api_key"; // TODO: Replace with actual API key
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<SearchResult> SearchNearbyBusinessesAsync(double latitude, double longitude, int radius)
    {
        try
        {
            Debug.WriteLine($"[BusinessService] Searching for businesses near ({latitude}, {longitude}) with radius {radius}m");
            
            // Simulating API delay
            await Task.Delay(500);
            
            Debug.WriteLine($"[BusinessService] Returning {_sampleBusinesses.Count} sample businesses:");
            foreach (var business in _sampleBusinesses)
            {
                Debug.WriteLine($"[BusinessService] - {business.Name} ({business.Category})");
            }
            
            return new SearchResult { Businesses = _sampleBusinesses };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[BusinessService] Error searching nearby businesses: {ex}");
            throw;
        }
    }

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
            await Task.Delay(200);
            
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