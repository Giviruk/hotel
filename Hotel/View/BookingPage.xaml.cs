using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotel.ViewModels;

namespace Hotel.View;

public partial class BookingPage : ContentPage
{
    private ReservationsPage _reservationsPage;
    
    public BookingPage(BookingViewModel vm,  RoomResponseDto availableRoom, DateTime startDate, DateTime endDate)
    {
        InitializeComponent();
        Title = "Book a room";
        vm.Init(availableRoom, startDate, endDate);
        vm.Navigation = Navigation;
        BindingContext = vm;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        NavigationPage.SetHasBackButton(this, true);
        NavigationPage.SetBackButtonTitle(this, "Назад");
    }

}