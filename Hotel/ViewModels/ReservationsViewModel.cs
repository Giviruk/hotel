using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Models;
using Hotel.Services;
using Hotel.View;

namespace Hotel.ViewModels;

public partial class ReservationsViewModel(
    HttpClient httpClient,
    IRsaEncryptionService rsaEncryptionService) : ObservableObject
{
    public ObservableCollection<ReservationDisplay> Reservations { get; } = [];

    public ICommand LoadCommand => new AsyncRelayCommand(LoadReservations);
    public ICommand GoToChangeStatusPopUpCommand => new AsyncRelayCommand<ReservationDisplay>(GoToChangeStatusPopUp);
    public INavigation Navigation { get; set; }

    public async Task LoadReservations()
    {
        if (SecureKeyStore.IsPinEntered)
        {
            await rsaEncryptionService.LoadKey();
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
                    await rsaEncryptionService.LoadKey();
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

        await rsaEncryptionService.LoadKey();

        try
        {
            var response = await httpClient.GetAsync("/hcont/api/v1/reservation");
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            var list = JsonSerializer.Deserialize<List<ReservationResponseDto>>(json);
            Reservations.Clear();
            if (list != null)
                foreach (var item in list)
                {
                    var contactInfo = EncodingHelper.Base64ToJson<string>(item.contactInfo);
                    var decryptedJson = rsaEncryptionService.Decrypt(contactInfo);
                    var contact = JsonSerializer.Deserialize<ContactInfo>(decryptedJson);
                    if (contact != null) Reservations.Add(new ReservationDisplay(item, contact));
                }
            /*var a = new ReservationDisplay(new ReservationResponseDto(1, 1, 12, 12, DateTime.Now, DateTime.Now,
                    "ewe",
                    "23", "23", "43"),
                new ContactInfo("2134", "2144", "2141", "wqe", "cz", DateTime.Now, "34344", DateTime.Now,
                    DateTime.Now, "erer"));
                
            Reservations.Clear();
            Reservations.Add(a);*/

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading reservations: {ex.Message}");
        }
    }

    public async Task GoToChangeStatusPopUp(ReservationDisplay reservation)
    {
        await Navigation.PushModalAsync(new UpdateReservationStatusPopup(reservation, httpClient));
    }

    public class ReservationDisplay(ReservationResponseDto dto, ContactInfo contact)
    {
        public long RoomId { get; } = dto.roomId;
        public string MainPerson { get; } = dto.mainPerson;
        public DateTime? StartDate { get; } = dto.StartDate;
        public DateTime? EndDate { get; } = dto.EndDate;
        public ContactInfo DecryptedContactInfo { get; } = contact;
    }
}