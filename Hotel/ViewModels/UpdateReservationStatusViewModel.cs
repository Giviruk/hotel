using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Models;
using Hotel.Models.Enums;

namespace Hotel.ViewModels;

public partial class UpdateReservationStatusViewModel(
    ReservationsViewModel.ReservationDisplay reservation,
    HttpClient httpClient) : ObservableObject
{
    [ObservableProperty] private long reservationId;
    [ObservableProperty] private RoomStatus selectedStatus;

    public ObservableCollection<RoomStatus> AvailableStatuses { get; } = new(
        Enum.GetValues(typeof(RoomStatus)).Cast<RoomStatus>());

    public ICommand SubmitStatusCommand => new AsyncRelayCommand(UpdateStatus);

    private async Task UpdateStatus()
    {
        try
        {
            var room =
                await httpClient.GetFromJsonAsync<RoomResponseDto>(
                    $"/hcont/api/v1/room/{reservation.RoomId}");
            room = room with { color = SelectedStatus };
            var json = JsonSerializer.Serialize(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PatchAsync($"/hcont/api/v1/room/{reservation.RoomId}", content);

            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Успех", "Статус обновлён", "OK");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось обновить статус", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }
}