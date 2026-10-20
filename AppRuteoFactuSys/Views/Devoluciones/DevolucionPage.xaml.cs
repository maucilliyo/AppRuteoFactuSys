namespace AppRuteoFactuSys.Views.Devoluciones;

public partial class DevolucionPage : ContentPage
{
	private readonly DevolucionViewModel viewModel;
    public DevolucionPage(DevolucionViewModel devolucionViewModel)
	{
		InitializeComponent();
		viewModel = devolucionViewModel;
		this.BindingContext = viewModel;
	}
    //VALIDA EL BOTON CERRAR DEL TELEFONO
    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            var response = await DisplayAlertAsync("AVISO", "¿Está seguro de salir sin guardar?", "Sí", "No");
            if (response)
            {
                await Navigation.PopAsync();
            }
        });

        // Devuelve true para indicar que has manejado el evento y que no debe cerrarse la página automáticamente
        return true;
    }
}