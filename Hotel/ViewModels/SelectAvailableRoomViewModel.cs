using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hotel.Models;
using Hotel.Models.Enums;
using Hotel.View;

namespace Hotel.ViewModels;

public partial class SelectAvailableRoomViewModel(HttpClient httpClient, BookingViewModel bookingVM) : ObservableObject
{
    [ObservableProperty] private DateTime _startDate = DateTime.Today;
    [ObservableProperty] private DateTime _endDate = DateTime.Today.AddDays(1);

    public INavigation Navigation { get; set; }
    public ObservableCollection<RoomResponseDto> AvailableRooms { get; } = new();

    public ICommand ShowAvailableRoomsCommand => new AsyncRelayCommand(LoadAvailableRooms);
    public ICommand BookRoomCommand => new AsyncRelayCommand<RoomResponseDto>(BookRoom);

    public async Task LoadAvailableRooms()
    {
        try
        {
            var response =
                await httpClient.GetFromJsonAsync<List<RoomResponseDto>>(
                    $"/hcont/api/v1/room");
            AvailableRooms.Clear();
            foreach (var room in response ?? new())
                AvailableRooms.Add(room);

            // mock
            /*var mockRooms = new List<RoomResponseDto>
            {
                new(1, 10, "Стандарт", RoomStatus.Available, "101"),
                new(2, 11, "Люкс", RoomStatus.Available, "102"),
                new(3, 12, "Апартаменты", RoomStatus.Booked, "103")
            };

            AvailableRooms.Clear();
            foreach (var room in mockRooms)
                AvailableRooms.Add(room);*/
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении списка комнат: {ex.Message}");
        }
    }

    private async Task BookRoom(RoomResponseDto room)
    {
        //if (room.color != RoomStatus.Available) return;
        var bookingPage = new BookingPage(bookingVM, room, StartDate, EndDate);
        await Navigation.PushAsync(bookingPage);
    }
}

public record RoomResponseDto(long id, long roomCategoryId, string categoryName, RoomStatus? color, string name);