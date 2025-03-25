using System;
using System.Collections.Generic;

namespace SmartSearch.Models
{
    public class Business
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> Photos { get; set; } = new();
        public BusinessHours Hours { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public bool IsOpen { get; set; }
        public double Distance { get; set; } // in kilometers
        public string PriceLevel { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public DateTime LastUpdated { get; set; }
        public List<string> Features { get; set; } = new List<string>();

        public string FormattedDistance => $"{Distance:F1}km";
        public string FormattedCategory => $"{Category} • {FormattedDistance}";
    }

    public class BusinessHours
    {
        public DayHours Monday { get; set; } = new();
        public DayHours Tuesday { get; set; } = new();
        public DayHours Wednesday { get; set; } = new();
        public DayHours Thursday { get; set; } = new();
        public DayHours Friday { get; set; } = new();
        public DayHours Saturday { get; set; } = new();
        public DayHours Sunday { get; set; } = new();
    }

    public class DayHours
    {
        public bool IsOpen { get; set; }
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;
    }
} 