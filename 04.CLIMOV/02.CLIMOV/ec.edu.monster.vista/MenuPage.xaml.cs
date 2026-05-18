using System;
using Microsoft.Maui.Controls;

namespace _02.CLIMOV.Vista
{
    public partial class MenuPage : ContentPage
    {
        private StackLayout _currentOpenForm = null;

        public MenuPage()
        {
            InitializeComponent();
            string username = Preferences.Get("username", "Usuario");
            LblUsuario.Text = username.Length > 0 ? username[0].ToString() : "M";
        }

        // Acordeón: Toggle Cards
        private void OnCardMovimientosClicked(object sender, EventArgs e) => ToggleCard(FormMovimientos);
        private void OnCardDepositoClicked(object sender, EventArgs e) => ToggleCard(FormDeposito);
        private void OnCardRetiroClicked(object sender, EventArgs e) => ToggleCard(FormRetiro);
        private void OnCardTransferenciaClicked(object sender, EventArgs e) => ToggleCard(FormTransferencia);

        private void ToggleCard(StackLayout form)
        {
            if (_currentOpenForm == form)
            {
                form.IsVisible = false;
                _currentOpenForm = null;
            }
            else
            {
                if (_currentOpenForm != null)
                    _currentOpenForm.IsVisible = false;
                form.IsVisible = true;
                _currentOpenForm = form;
            }
        }

        private async void OnConsultarMovimientosClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MovimientosPage");
        }

        private async void OnDepositoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//DepositoPage");
        }

        private async void OnRetiroClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RetiroPage");
        }

        private async void OnTransferenciaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TransferenciaPage");
        }

        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            bool confirmacion = await DisplayAlert(
                "Cerrar Sesión",
                "¿Está seguro de que desea cerrar sesión?",
                "Sí",
                "No");

            if (confirmacion)
            {
                Preferences.Remove("isLoggedIn");
                Preferences.Remove("username");
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
