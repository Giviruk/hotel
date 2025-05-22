using System.Text;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Models;
using Hotel.Services;

namespace Hotel.ViewModels;

public partial class BookingViewModel : ObservableObject
{
    [ObservableProperty] private long roomId;
    [ObservableProperty] private float price;
    [ObservableProperty] private float cost;
    [ObservableProperty] private DateTime startDate = DateTime.Today;
    [ObservableProperty] private DateTime endDate = DateTime.Today.AddDays(1);
    [ObservableProperty] private string comment;
    [ObservableProperty] private string mainPerson;
    [ObservableProperty] private string contactEmail;

    // contact_info как поля формы
    [ObservableProperty] private string phone;
    [ObservableProperty] private string email;
    [ObservableProperty] private string fio;
    [ObservableProperty] private string sex;
    [ObservableProperty] private string citizenship;
    [ObservableProperty] private DateTime birthday = DateTime.Today.AddYears(-20);
    [ObservableProperty] private string passportNumber;
    [ObservableProperty] private DateTime passportDate = DateTime.Today.AddYears(-5);
    [ObservableProperty] private DateTime passDateEnd = DateTime.Today.AddYears(5);
    [ObservableProperty] private string registration;

    [ObservableProperty] private string status;

    private readonly HttpClient _httpClient;
    
    private readonly IRsaEncryptionService _rsa;

    public BookingViewModel(HttpClient httpClient, IRsaEncryptionService rsa)
    {
        _httpClient = httpClient;
        _rsa = rsa;
    }

    public ICommand SubmitCommand => new AsyncRelayCommand(SubmitBooking);

    private async Task SubmitBooking()
    {
        Status = string.Empty;

        var contactInfo = new ContactInfo(
            Phone, Email, Fio, Sex, Citizenship,
            Birthday, PassportNumber, PassportDate, PassDateEnd, Registration);

        string contactInfoJson = JsonSerializer.Serialize(contactInfo);
        string contactInfoEncrypted = _rsa.Encrypt(contactInfoJson);

        var request = new ReservationRequestDto(
            RoomId, Price, Cost, StartDate, EndDate,
            Comment, MainPerson, contactInfoEncrypted, ContactEmail);

        var json = JsonSerializer.Serialize(request);
        
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/book-table", content);
            Status = response.IsSuccessStatusCode ? "Booking successful" : "Booking failed: " + response.StatusCode;
        }
        catch (Exception ex)
        {
            Status = "Error: " + ex.Message;
        }
    }
}