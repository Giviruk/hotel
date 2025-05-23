using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Models;
using Hotel.Services;

namespace Hotel.ViewModels;

public partial class ReservationsViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private readonly IRsaEncryptionService _rsa;

    public ObservableCollection<ReservationDisplay> Reservations { get; } = new();

    public ICommand LoadCommand => new AsyncRelayCommand(LoadReservations);

    public ReservationsViewModel(HttpClient httpClient, IRsaEncryptionService rsa)
    {
        _httpClient = httpClient;
        _rsa = rsa;
    }

    public async Task LoadReservations()
    {
        try
        {
            var response = await _httpClient.GetAsync("/hcont/api/v1/reservation");
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
                var list = JsonSerializer.Deserialize<List<ReservationResponseDto>>(json);
            Reservations.Clear();
            foreach (var item in list)
            {
                var decryptedJson = _rsa.Decrypt(item.contactInfo);
                var contact = JsonSerializer.Deserialize<ContactInfo>(decryptedJson);
                Reservations.Add(new ReservationDisplay(item, contact));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading reservations: {ex.Message}");
        }
    }

    public class ReservationDisplay
    {
        public long RoomId { get; }
        public string MainPerson { get; }
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }
        public ContactInfo DecryptedContactInfo { get; }

        public ReservationDisplay(ReservationResponseDto dto, ContactInfo contact)
        {
            RoomId = dto.roomId;
            MainPerson = dto.mainPerson;
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
            DecryptedContactInfo = contact;
        }
    }
}