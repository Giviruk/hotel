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

        var container = new CookieContainer();
        
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = container,
        };
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://10.0.2.2:8090")
        };
        builder.Services.AddSingleton(client);
        builder.Services.AddSingleton(container);
        builder.Services.AddSingleton(handler);

        return builder.Build();
    }
}