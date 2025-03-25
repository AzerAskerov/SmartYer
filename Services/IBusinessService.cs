using SmartSearch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSearch.Services
{
    public interface IBusinessService
    {
        Task<SearchResult> SearchNearbyBusinessesAsync(double latitude, double longitude, int radius);
        Task<List<string>> GetPopularCategoriesAsync();
        Task<List<Business>> GetRecentlyVisitedAsync();
        Task<List<string>> GetSuggestedCategoriesAsync(string query);
    }
} 