using Acr.UserDialogs;
using MobileDbBenchmark.iOS;
using MobileDbBenchmark.UI;

[assembly: Xamarin.Forms.Dependency(typeof(DialogService))]
namespace MobileDbBenchmark.iOS
{
    public class DialogService : IDialogService
    {
        public void ShowProgressDialog()
        {
            // UserDialogs.Instance.ShowLoading("Wykonuje test");

        }

        public void HideProgressDialog()
        {
            // UserDialogs.Instance.HideLoading();
        }
    }
}