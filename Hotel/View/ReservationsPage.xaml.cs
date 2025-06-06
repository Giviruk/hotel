using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotel.ViewModels;

namespace Hotel.View;

public partial class ReservationsPage : ContentPage
{
    private readonly ReservationsViewModel reservationsViewModel;
    public ReservationsPage(ReservationsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        reservationsViewModel = vm;
        vm.Navigation = Navigation; // <-- добавь это
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        reservationsViewModel.LoadCommand.Execute(null);
    }
}