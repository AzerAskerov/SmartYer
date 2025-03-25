using SmartSearch.Services;
using SmartSearch.ViewModels;

namespace SmartSearch;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

	public MainPage(IBusinessService businessService, ILocationService locationService)
	{
		InitializeComponent();
		_viewModel = new MainViewModel(businessService, locationService);
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
		await _viewModel.StartLocationTrackingCommand.ExecuteAsync(null);
	}

	protected override async void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.Cleanup();
		await _viewModel.StopLocationTrackingCommand.ExecuteAsync(null);
	}
}

