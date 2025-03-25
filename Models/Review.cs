using System;
using System.Collections.Generic;

namespace SmartSearch.Models
{
    public class Review
    {
        public string Id { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserPhotoUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Photos { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerified { get; set; }
        public bool IsHelpful { get; set; }
        public int HelpfulCount { get; set; }
        public List<string> Tags { get; set; } = new();
        public string VisitDate { get; set; } = string.Empty;
        public string VisitType { get; set; } = string.Empty;
        public Dictionary<string, double> CategoryRatings { get; set; } = new();

        public Review()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
} 