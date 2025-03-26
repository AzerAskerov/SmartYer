using SmartSearch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSearch.Services
{
    public interface IBusinessService
    {
        Task<List<Business>> GetRecentBusinessesAsync();
        Task<List<Business>> GetLocalBusinessesAsync(double latitude, double longitude);
        Task<List<Business>> SearchNearbyBusinessesAsync(double latitude, double longitude);
        Task<List<string>> GetPopularCategoriesAsync();
        Task<List<Business>> GetRecentlyVisitedAsync();
        Task<List<string>> GetSuggestedCategoriesAsync(string query);
        Task<Business> GetBusinessDetailsAsync(string businessId);
        Task<List<Review>> GetBusinessReviewsAsync(string businessId, int page = 1, int pageSize = 20);
        Task<Review> CreateReviewAsync(string businessId, Review review);
        Task<Review> UpdateReviewAsync(string businessId, string reviewId, Review review);
        Task DeleteReviewAsync(string businessId, string reviewId);
        Task MarkReviewHelpfulAsync(string businessId, string reviewId);
    }
} 