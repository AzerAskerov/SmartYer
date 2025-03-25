using System;
using System.Collections.Generic;

namespace SmartSearch.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public bool IsVerified { get; set; }
    public string PreferredLanguage { get; set; } = string.Empty;
    public List<string> FavoriteCategories { get; set; } = new();
    public List<string> FavoriteBusinesses { get; set; } = new();
    public List<string> RecentSearches { get; set; } = new();
    public Dictionary<string, object> Preferences { get; set; } = new();
    public int ReviewCount { get; set; }
    public int PhotoCount { get; set; }
    public int HelpfulVotes { get; set; }
    public string UserType { get; set; } = string.Empty;
    public bool IsBusinessOwner { get; set; }
    public List<string> OwnedBusinesses { get; set; } = new();
    public List<string> FollowedUsers { get; set; } = new();
    public List<string> Followers { get; set; } = new();
    public List<string> Notifications { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
} 