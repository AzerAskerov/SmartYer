using SmartSearch.Models;

namespace SmartSearch.Views;

[QueryProperty(nameof(Business), "Business")]
public partial class BusinessDetailsPage : ContentPage
{
    private Business? _business;
    public Business? Business
    {
        get => _business;
        set
        {
            _business = value;
            UpdateUI();
        }
    }

    public BusinessDetailsPage()
    {
        InitializeComponent();
    }

    private void UpdateUI()
    {
        if (_business == null) return;

        BusinessImage.Source = _business.ImageUrl;
        BusinessName.Text = _business.Name;
        Category.Text = _business.Category;
        Rating.Text = $"{_business.Rating:F1} ★";
        ReviewCount.Text = $"({_business.ReviewCount} reviews)";
    }
} 