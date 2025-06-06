using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Services;
using Hotel.View;

namespace Hotel.ViewModels;

public partial class LoginViewModel(
    HttpClient httpClient,
    SelectAvailableRoomPage selectAvailableRoomPage,
    CookieContainer cookieContainer)
    : ObservableObject
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string status = string.Empty;

    public INavigation Navigation { get; set; }

    public ICommand LoginCommand => new AsyncRelayCommand(LoginAsync);

    private async Task LoginAsync()
    {
        Status = string.Empty;

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("username", Username),
            new KeyValuePair<string, string>("password", Password)
        ]);
        
        try
        {
            var response = await httpClient.PostAsync("/usercheck", content);// Авторизация прошла успешно
            var cookies = cookieContainer.GetCookies(new Uri("http://10.0.2.2:8090"));

            if (response.RequestMessage != null && response.RequestMessage.RequestUri != null && !response.RequestMessage.RequestUri.Query.Contains("error"))
            //if(true)
            {
                Status = "Login successful";
                Application.Current.MainPage = new NavigationPage(selectAvailableRoomPage);
            }
            else
            {
                Status = "Login failed: "; //+ response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            Status = "Error: " + ex.Message;
        }
    }
}