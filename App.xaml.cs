using Microsoft.Extensions.DependencyInjection;
using SmartSearch.ViewModels;

namespace SmartSearch;

public partial class App : Application
{
	private readonly IServiceProvider _serviceProvider;
	private MainViewModel? _mainViewModel;

	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_serviceProvider = serviceProvider;
		MainPage = new AppShell();
	}

	protected override async void OnStart()
	{
		base.OnStart();
		_mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
		await _mainViewModel.InitializeAsync();
	}

	protected override void OnSleep()
	{
		base.OnSleep();
		_mainViewModel?.Cleanup();
	}

	protected override void OnResume()
	{
		base.OnResume();
		_ = _mainViewModel?.InitializeAsync();
	}
}