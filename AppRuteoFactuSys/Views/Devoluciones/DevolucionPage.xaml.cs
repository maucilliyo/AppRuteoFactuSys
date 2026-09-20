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
}