using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;

namespace EncryptingAndDecryptingMobileApp;

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
			});

		builder.Services.AddScoped<IFileSaver>(_ => FileSaver.Default);
		builder.Services.AddTransient<MainPage>();

		return builder.Build();
	}
}
