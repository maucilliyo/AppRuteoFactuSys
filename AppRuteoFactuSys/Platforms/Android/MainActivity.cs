using Android.App;
using Android.Content.PM;
using Android.OS;

namespace AppRuteoFactuSys
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Resto de tu código aquí

            // Bloquear la orientación de la pantalla en posición vertical
            RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}
