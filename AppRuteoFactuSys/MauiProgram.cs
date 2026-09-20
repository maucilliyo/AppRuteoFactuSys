using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;
using AppRuteoFactuSys.Views;
using AppRuteoFactuSys.Views.Devoluciones;
using Controls.UserDialogs.Maui;
using Microsoft.Extensions.Logging;

namespace AppRuteoFactuSys
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUserDialogs(true, () =>
                {
#if ANDROID
                    var fontFamily = "OpenSans-Regular.ttf";
#else
                    var fontFamily = "OpenSans-Regular";
#endif
                    AlertConfig.DefaultMessageFontFamily = fontFamily;
                    AlertConfig.DefaultUserInterfaceStyle = UserInterfaceStyle.Dark;
                    AlertConfig.DefaultPositiveButtonTextColor = Colors.Purple;
                    ConfirmConfig.DefaultMessageFontFamily = fontFamily;
                    ActionSheetConfig.DefaultMessageFontFamily = fontFamily;
                    ToastConfig.DefaultMessageFontFamily = fontFamily;
                    SnackbarConfig.DefaultMessageFontFamily = fontFamily;
                    HudDialogConfig.DefaultMessageFontFamily = fontFamily;
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            //dependencias
            builder.Services.AddTransient<ViewFactoryServices>();



            // builder.Services.AddDbContext<ConexionDb>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<SincronizarPage>();
            builder.Services.AddTransient<ListaProductosPage>();
            builder.Services.AddTransient<PreventaPage>(); //VerFacturaPage
            builder.Services.AddTransient<VerFacturaPage>();
            builder.Services.AddTransient<MenuPreventaPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ListaDevolucionesPage>();
            builder.Services.AddTransient<DevolucionPage>();
            builder.Services.AddTransient<ClienteAdd2Page>();
            builder.Services.AddTransient<ProductoAdd2Page>();
            builder.Services.AddTransient<CantidadEditPage>();

            //
            builder.Services.AddTransient<DevolucionViewModel>();

            //
            builder.Services.AddScoped<IClienteService, ClienteService>();
            builder.Services.AddSingleton<IPreventaService, PreventaService>();
            builder.Services.AddSingleton<IProductoService, ProductoService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<DevolucionService>();
            //MySql
            builder.Services.AddSingleton<PreventaRepository>();
            builder.Services.AddSingleton<ClienteRepository>();
            builder.Services.AddSingleton<ProductoRepository>();
            //SQL LITE
            builder.Services.AddSingleton<LocalPreventaRepository>();
            builder.Services.AddSingleton<LocalClienteRepository>();
            builder.Services.AddSingleton<LocalProductoRepository>();
            builder.Services.AddSingleton<LocalDevolucionRepository>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

    }
}
