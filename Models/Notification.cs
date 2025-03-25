using System;

namespace SmartSearch.Models;

public class Notification
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsArchived { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? RelatedBusinessId { get; set; }
    public string? RelatedReviewId { get; set; }
    public string? RelatedUserId { get; set; }
    public DateTime? ExpiresAt { get; set; }
} 