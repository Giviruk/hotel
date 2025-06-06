using System.Text;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Models;
using Hotel.Services;

namespace Hotel.ViewModels;

public partial class BookingViewModel(HttpClient httpClient, IRsaEncryptionService rsa) : ObservableObject
{
    [ObservableProperty] private long roomId;
    [ObservableProperty] private float price;
    [ObservableProperty] private float cost;
    [ObservableProperty] private DateTime startDate = DateTime.Today;
    [ObservableProperty] private DateTime endDate = DateTime.Today.AddDays(1);
    [ObservableProperty] private string comment = string.Empty;
    [ObservableProperty] private string mainPerson = string.Empty;
    [ObservableProperty] private string contactEmail = string.Empty;

    // contact_info как поля формы
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string fio = string.Empty;
    [ObservableProperty] private string sex = string.Empty;
    [ObservableProperty] private string citizenship = string.Empty;
    [ObservableProperty] private DateTime birthday = DateTime.Today.AddYears(-20);
    [ObservableProperty] private string passportNumber = string.Empty;
    [ObservableProperty] private DateTime passportDate = DateTime.Today.AddYears(-5);
    [ObservableProperty] private DateTime passDateEnd = DateTime.Today.AddYears(5);
    [ObservableProperty] private string registration = string.Empty;

    [ObservableProperty] private string status = string.Empty;
    
    public INavigation Navigation { get; set; }

    public ICommand SubmitCommand => new AsyncRelayCommand(SubmitBooking);

    public void Init(RoomResponseDto availableRoom, DateTime choosenStartDate, DateTime choosenEndDate)
    {
        roomId = availableRoom.id;
        startDate = choosenStartDate;
        endDate = choosenEndDate;
        
    }

    private async Task SubmitBooking()
    {
        Status = string.Empty;
        
        if (SecureKeyStore.IsPinEntered)
        {
            await rsa.LoadKey();
        }
        else
        {
            var pin = await Application.Current.MainPage?.DisplayPromptAsync(
                "Введите пин-код",
                "Введите ваш личный код доступа",
                accept: "Сохранить",
                cancel: "Отмена",
                maxLength: 4096,
                keyboard: Keyboard.Text);

            var isPinVerified = await SecureKeyStore.HasPrivateKeyAsync(pin);
            if (!isPinVerified)
            {
                var res = await Application.Current.MainPage.DisplayActionSheet(
                    "Код доступа не верен. Сгенерировать новый ключ доступа?", "Да", "Нет");
                if (res == "Да")
                {
                    await SecureKeyStore.EnterPin(pin);
                    await rsa.LoadKey();
                }
                else
                {
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await SecureKeyStore.EnterPin(pin);
            }
        }
        
        await rsa.LoadKey();
        

        var contactInfo = new ContactInfo(
            Phone, Email, Fio, Sex, Citizenship,
            Birthday, PassportNumber, PassportDate, PassDateEnd, Registration);

        string contactInfoJson = JsonSerializer.Serialize(contactInfo);
        string contactInfoEncrypted = rsa.Encrypt(contactInfoJson);
        
        var base64 = EncodingHelper.JsonToBase64(contactInfoEncrypted);

        var request = new ReservationRequestDto(
            RoomId, Price, Cost, StartDate, EndDate,
            Comment, MainPerson, base64, ContactEmail);

        var json = JsonSerializer.Serialize(request);
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("/hcont/api/v1/reservation", content);
            Status = response.IsSuccessStatusCode ? "Booking successful" : "Booking failed: " + response.StatusCode;
        }
        catch (Exception ex)
        {
            Status = "Error: " + ex.Message;
        }
    }
}