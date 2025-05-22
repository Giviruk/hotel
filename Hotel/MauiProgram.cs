using System.Net;
using Hotel.Services;
using Hotel.View;
using Hotel.ViewModels;
using Microsoft.Extensions.Logging;

namespace Hotel;

public static class  MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<BookingPage>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<BookingViewModel>();
        builder.Services.AddSingleton<ReservationsPage>();
        builder.Services.AddSingleton<ReservationsViewModel>();
        builder.Services.AddSingleton<IRsaEncryptionService, RsaEncryptionService>();

        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer(),
        };
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://your.api")
        };
        builder.Services.AddSingleton(client);


        return builder.Build();
    }
}