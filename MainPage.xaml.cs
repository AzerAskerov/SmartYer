using SmartSearch.Services;
using SmartSearch.ViewModels;

namespace SmartSearch;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.Cleanup();
	}
}

