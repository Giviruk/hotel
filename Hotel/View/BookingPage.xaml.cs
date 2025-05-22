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
    
    public BookingPage(BookingViewModel vm, ReservationsPage reservationsPage)
    {
        InitializeComponent();
        Title = "Book a room";
        BindingContext = vm;
        
        _reservationsPage = reservationsPage;
        
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Мои брони",
            Priority = 0,
            Order = ToolbarItemOrder.Primary,
            Command = new Command(async () => await Navigation.PushAsync(_reservationsPage))
        });
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        NavigationPage.SetHasBackButton(this, true);
        NavigationPage.SetBackButtonTitle(this, "Назад");
    }

}