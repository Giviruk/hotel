using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotel.ViewModels;

namespace Hotel.View;

public partial class ReservationsPage : ContentPage
{
    ReservationsViewModel _reservationsViewModel;
    public ReservationsPage(ReservationsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _reservationsViewModel = vm;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(() => _reservationsViewModel.LoadReservations());
    }
}