using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartSearch.Services;
using SmartSearch.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging.Debug;

namespace SmartSearch;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Lucide.ttf", "Lucide");
			});

		// Configure services
		builder.Services.AddHttpClient();

		// Register Services
		builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
		builder.Services.AddSingleton<IBusinessService, BusinessService>();
		builder.Services.AddSingleton<ILocationService, LocationService>();

		// Register ViewModels
		builder.Services.AddTransient<MainViewModel>();

		// Register Pages
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<Views.BusinessDetailsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
