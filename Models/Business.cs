using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SmartSearch.Models
{
    public partial class Business : ObservableObject
    {
        [ObservableProperty]
        private string _id = string.Empty;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _category = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _address = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private string _website = string.Empty;

        [ObservableProperty]
        private string _imageUrl = string.Empty;

        [ObservableProperty]
        private BusinessLocation _location = new();

        [ObservableProperty]
        private double _rating;

        [ObservableProperty]
        private int _reviewCount;

        [ObservableProperty]
        private List<string> _tags = new();

        [ObservableProperty]
        private List<string> _photos = new();

        [ObservableProperty]
        private BusinessHours _hours = new();

        [ObservableProperty]
        private List<Review> _reviews = new();

        [ObservableProperty]
        private bool _isOpen;

        [ObservableProperty]
        private double _distance;

        [ObservableProperty]
        private string _priceLevel = string.Empty;

        [ObservableProperty]
        private List<string> _amenities = new();

        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.Now;

        [ObservableProperty]
        private List<string> _features = new();

        [ObservableProperty]
        private bool _isFromGooglePlaces;

        [ObservableProperty]
        private bool _isFavorite;

        [RelayCommand]
        private void ToggleFavorite()
        {
            IsFavorite = !IsFavorite;
        }

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