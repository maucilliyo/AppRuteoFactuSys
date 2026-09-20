using System.Globalization;

namespace AppRuteoFactuSys.Views
{
    public partial class CantidadEditPage : ContentPage
    {
        private readonly TaskCompletionSource<decimal?> _tcs = new();

        public Task<decimal?> CantidadTask => _tcs.Task;

        public CantidadEditPage()
        {
            InitializeComponent();
        }

        /// <summary>Llamar antes de PushModalAsync.</summary>
        public void Configurar(string descripcion, decimal cantidadActual)
        {
            lblDescripcion.Text = descripcion;
            txtCantidad.Text = cantidadActual.ToString("0.###", CultureInfo.InvariantCulture);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Foco y todo el texto seleccionado para reemplazar rápido
            Dispatcher.Dispatch(() =>
            {
                txtCantidad.Focus();
                txtCantidad.CursorPosition = 0;
                txtCantidad.SelectionLength = txtCantidad.Text?.Length ?? 0;
            });
        }

        private bool TryLeerCantidad(out decimal cantidad)
        {
            // Acepta coma o punto como separador decimal
            var texto = (txtCantidad.Text ?? string.Empty).Trim().Replace(',', '.');
            return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out cantidad);
        }

        private void Ajustar(decimal delta)
        {
            TryLeerCantidad(out var cantidad);
            cantidad = Math.Max(cantidad + delta, 0);
            txtCantidad.Text = cantidad.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private void btnMas_Clicked(object sender, EventArgs e) => Ajustar(1);

        private void btnMenos_Clicked(object sender, EventArgs e) => Ajustar(-1);

        private async void btnAceptar_Clicked(object sender, EventArgs e)
        {
            if (!TryLeerCantidad(out var cantidad) || cantidad <= 0)
            {
                await DisplayAlert("Aviso", "INGRESE UNA CANTIDAD VALIDA", "Aceptar");
                return;
            }

            _tcs.TrySetResult(cantidad);
            await Navigation.PopModalAsync();
        }

        private void txtCantidad_Completed(object sender, EventArgs e)
            => btnAceptar_Clicked(sender, e);

        private async void btnCancelar_Clicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult(null);
            await Navigation.PopModalAsync();
        }

        protected override void OnDisappearing()
        {
            // Cubre botón atrás y cualquier otra forma de cerrar sin aceptar.
            // Si ya se resolvió con Aceptar, TrySetResult no hace nada.
            _tcs.TrySetResult(null);
            base.OnDisappearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult(0);
            await Navigation.PopModalAsync();
        }
    }
}