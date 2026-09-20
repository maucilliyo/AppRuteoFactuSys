using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Service
{
    public interface IDialogService
    {
        Task ShowAlert(string titulo, string mensaje);
        Task<bool> ShowConfirm(string titulo, string mensaje, string si = "Sí", string no = "No");
    }

    public class DialogService : IDialogService
    {
        public Task ShowAlert(string titulo, string mensaje)
        {
            return Application.Current?.MainPage?.DisplayAlert(titulo, mensaje, "OK")
                   ?? Task.CompletedTask;
        }

        public Task<bool> ShowConfirm(string titulo, string mensaje, string si = "Sí", string no = "No")
        {
            var page = Application.Current?.MainPage;
            if (page is null)
                return Task.FromResult(false);

            // Garantiza que el diálogo se muestre en el hilo de UI
            return MainThread.InvokeOnMainThreadAsync(() => page.DisplayAlert(titulo, mensaje, si, no));
        }
    }
}
