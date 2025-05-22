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

    public LoginViewModel(HttpClient httpClient, BookingPage bookingPage)
    {
        _httpClient = httpClient;
        _bookingPage = bookingPage;
    }

    public ICommand LoginCommand => new AsyncRelayCommand(LoginAsync);

    private async Task LoginAsync()
    {
        Status = string.Empty;

        var payload = new { username = Username, password = Password };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/login", content);

            if (response.IsSuccessStatusCode)
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

                await Navigation.PushAsync(_bookingPage);
            }
            else
            {
                //Status = "Login failed: " + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            Status = "Error: " + ex.Message;
        }
    }
}