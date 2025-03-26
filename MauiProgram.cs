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

		// Load configuration
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

		// Configure Google Places API
		var googleApiKey = configuration.GetSection("GooglePlaces:ApiKey").Value;
		if (string.IsNullOrEmpty(googleApiKey))
		{
			googleApiKey = "AIzaSyCkFb29hN8XJiGjo2B2YctH4w2vy0L9LaY"; // Fallback to default key
		}
		builder.Services.AddSingleton<IGooglePlacesService>(sp => 
			new GooglePlacesService(sp.GetRequiredService<IHttpClientFactory>().CreateClient(), googleApiKey));

		// Register Services
		builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
		builder.Services.AddSingleton<IWifiService, WifiService>();
		builder.Services.AddSingleton<ILocationService, LocationService>();
		builder.Services.AddSingleton<IBusinessService, BusinessService>();

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
