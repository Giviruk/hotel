using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotel.ViewModels;

namespace Hotel.View;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        vm.Navigation = Navigation; // <-- добавь это
        BindingContext = vm;
    }
}