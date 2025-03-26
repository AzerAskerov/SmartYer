using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartSearch.Models;
using System.Diagnostics;

namespace SmartSearch.Services;

public interface IGooglePlacesService
{
    Task<List<Business>> SearchNearbyPlacesAsync(double latitude, double longitude, int radius);
}

public class GooglePlacesService : IGooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/place";

    public GooglePlacesService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<List<Business>> SearchNearbyPlacesAsync(double latitude, double longitude, int radius)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Debug.WriteLine("[GooglePlacesService] API key is not configured");
                return new List<Business>();
            }

            // Use valid Google Places types for businesses
            var types = new[] { "restaurant", "store", "shopping_mall" }; // Simplified list of types
            var typeString = string.Join("|", types);
            var url = $"{BaseUrl}/nearbysearch/json?location={latitude},{longitude}&radius=5000&key={_apiKey}";
            
            Debug.WriteLine($"[GooglePlacesService] Searching for businesses near {latitude}, {longitude} with URL: {url}");
            
            try
            {
                var response = await _httpClient.GetFromJsonAsync<GooglePlacesResponse>(url);

                if (response == null)
                {
                    Debug.WriteLine("[GooglePlacesService] Response was null");
                    return new List<Business>();
                }

                if (response.Status == "REQUEST_DENIED")
                {
                    Debug.WriteLine("[GooglePlacesService] API request denied - check API key");
                    return new List<Business>();
                }

                if (response.Status != "OK" && response.Status != "ZERO_RESULTS")
                {
                    Debug.WriteLine($"[GooglePlacesService] API returned status: {response.Status}");
                    return new List<Business>();
                }

                if (response.Results == null || response.Results.Count == 0)
                {
                    Debug.WriteLine("[GooglePlacesService] No results found");
                    return new List<Business>();
                }

                Debug.WriteLine($"[GooglePlacesService] Found {response.Results.Count} places");
                return response.Results.Select(place => new Business
                {
                    Id = place.PlaceId,
                    Name = place.Name,
                    Address = place.Vicinity,
                    Rating = place.Rating,
                    ReviewCount = place.UserRatingsTotal,
                    Category = place.Types?.FirstOrDefault() ?? "Uncategorized",
                    Location = new BusinessLocation
                    {
                        Latitude = place.Geometry.Location.Lat,
                        Longitude = place.Geometry.Location.Lng,
                        Address = place.Vicinity
                    },
                    Photos = place.Photos?.Select(p => 
                        $"{BaseUrl}/photo?maxwidth=400&photoreference={p.PhotoReference}&key={_apiKey}")
                        .ToList() ?? new List<string>(),
                    IsFromGooglePlaces = true,
                    Distance = 0 // Will be calculated by BusinessService
                }).ToList();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"[GooglePlacesService] Network error searching nearby places: {ex.Message}");
                return new List<Business>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[GooglePlacesService] Error parsing response: {ex.Message}");
                return new List<Business>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[GooglePlacesService] Error searching nearby places: {ex}");
            return new List<Business>();
        }
    }
}

public class GooglePlacesResponse
{
    [JsonPropertyName("results")]
    public List<GooglePlace> Results { get; set; } = new();

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class GooglePlace
{
    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("vicinity")]
    public string Vicinity { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("user_ratings_total")]
    public int UserRatingsTotal { get; set; }

    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; } = new();

    [JsonPropertyName("photos")]
    public List<Photo> Photos { get; set; } = new();
}

public class Geometry
{
    [JsonPropertyName("location")]
    public LatLngLiteral Location { get; set; } = new();
}

public class LatLngLiteral
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

public class Photo
{
    [JsonPropertyName("photo_reference")]
    public string PhotoReference { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
} 