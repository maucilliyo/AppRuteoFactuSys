namespace AppRuteoFactuSys
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            MainPage = new AppShell();

        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            //minstras este activa la app no se suspende el telefono
            Window window = base.CreateWindow(activationState);

            window.Created += (s, e) =>
            {
                DeviceDisplay.KeepScreenOn = true;
            };

            return window;
        }
    }
}
