using AppRuteoFactuSys.Models;
using CommunityToolkit.Maui.Core.Platform;

namespace AppRuteoFactuSys.Views;

public partial class EditarLineaPage : ContentPage
{
	PreventaLineas _linea;
    public EditarLineaPage(PreventaLineas linea)
	{
		InitializeComponent();
		_linea = linea;
		LoadProducto();
    }
	private void LoadProducto()
	{
		lblProducto.Text = _linea.Detalle; 
        txtCantidad.Text = _linea.Cantidad.ToString();
        lblPrecio.Text = (_linea.PrecioUnidad * (1 + _linea.Porimpuesto)).ToString("N2");
	}

    private void txtCantidad_Unfocused(object sender, FocusEventArgs e)
    {
        string previousText = string.Empty;

        txtCantidad.Focused += (s, e) =>
        {
            // Almacenar el valor actual cuando se enfoca el Entry
            previousText = txtCantidad.Text;
        };

        txtCantidad.Unfocused += (s, e) =>
        {
            if (!string.IsNullOrEmpty(txtCantidad.Text))
            {
                // Verificar si el valor es numérico
                if (!double.TryParse(txtCantidad.Text, out double result))
                {
                    // Si no es numérico, revertir al valor anterior
                    txtCantidad.Text = previousText;
                }
                else
                {
                    // Verificar si el valor es menor que 1
                    if (result < 1)
                    {
                        // Si es menor que 1, establecerlo a 1
                        txtCantidad.Text = "1";
                    }
                }
            }
        };

    }

    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        _linea.Cantidad = Convert.ToDecimal(txtCantidad.Text);
        PreventaPage.load.ModificarLinea(_linea,"mod");

        if (KeyboardExtensions.IsSoftKeyboardShowing(txtCantidad))
        {
            await KeyboardExtensions.HideKeyboardAsync(txtCantidad, default);
        }
        await Navigation.PopAsync();
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        PreventaPage.load.ModificarLinea(_linea, "delete");
        await Navigation.PopAsync();
    }
}