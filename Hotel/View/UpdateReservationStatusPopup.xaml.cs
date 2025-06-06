using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotel.Models.Enums;
using Hotel.ViewModels;

namespace Hotel.View;

public partial class UpdateReservationStatusPopup : ContentPage
{
    public UpdateReservationStatusPopup(ReservationsViewModel.ReservationDisplay reservationId, HttpClient httpClient)
    {
        InitializeComponent();
        BindingContext = new UpdateReservationStatusViewModel(reservationId, httpClient);
    }
}