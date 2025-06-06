using Hotel.ViewModels;

namespace Hotel.View;

public partial class SelectAvailableRoomPage : ContentPage
{
    public SelectAvailableRoomPage(SelectAvailableRoomViewModel vm, ReservationsPage reservationsPage)
    {
        InitializeComponent();
        vm.Navigation = Navigation; // <-- добавь это
        BindingContext = vm;
        
                
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Мои брони",
            Priority = 0,
            Order = ToolbarItemOrder.Primary,
            Command = new Command(async void () => await Navigation.PushAsync(reservationsPage))
        });
    }

    private void RoomsView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is RoomResponseDto selectedRoom &&
            BindingContext is SelectAvailableRoomViewModel vm)
        {
            vm.BookRoomCommand.Execute(selectedRoom);
        }
    }
}