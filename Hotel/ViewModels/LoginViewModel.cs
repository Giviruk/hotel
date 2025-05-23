using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Services;
using Hotel.View;

namespace Hotel.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string username;
    [ObservableProperty] private string password;
    [ObservableProperty] private string status;

    public INavigation Navigation { get; set; }

    private readonly HttpClient _httpClient;
    private readonly BookingPage _bookingPage;
    private readonly CookieContainer _cookieContainer;
    private readonly IRsaEncryptionService rsaEncryptionService;

    public LoginViewModel(HttpClient httpClient, BookingPage bookingPage, CookieContainer cookieContainer, IRsaEncryptionService rsaEncryptionService)
    {
        _httpClient = httpClient;
        _bookingPage = bookingPage;
        _cookieContainer = cookieContainer;
        this.rsaEncryptionService = rsaEncryptionService;
    }

    public ICommand LoginCommand => new AsyncRelayCommand(LoginAsync);

    private async Task LoginAsync()
    {
        Status = string.Empty;

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });
        
        try
        {
            var response = await _httpClient.PostAsync("/usercheck", content);// Авторизация прошла успешно
            var cookies = _cookieContainer.GetCookies(new Uri("http://10.0.2.2:8090"));

            if (!response.RequestMessage.RequestUri.Query.Contains("error"))
            {
                Status = "Login successful";

                if (!await SecureKeyStore.HasPrivateKeyAsync(username))
                {
                    string inputKey = await Application.Current.MainPage.DisplayPromptAsync(
                        "Приватный ключ",
                        "Введите ваш приватный ключ в формате PEM (BEGIN/END PRIVATE KEY):",
                        accept: "Сохранить",
                        cancel: "Отмена",
                        placeholder: "-----BEGIN PRIVATE KEY-----...-----END PRIVATE KEY-----",
                        maxLength: 4096,
                        keyboard: Keyboard.Text);

                    if (string.IsNullOrWhiteSpace(inputKey) || !inputKey.Contains("BEGIN PRIVATE KEY"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Неверный формат ключа.", "OK");
                        return;
                    }

                    await SecureKeyStore.SavePrivateKeyAsync(inputKey);
                }

                await rsaEncryptionService.LoadKey();
                await Navigation.PushAsync(_bookingPage);
            }
            else
            {
                Status = "Login failed: " + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            Status = "Error: " + ex.Message;
        }
    }
}