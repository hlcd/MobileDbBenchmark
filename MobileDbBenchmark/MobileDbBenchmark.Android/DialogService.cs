using Acr.UserDialogs;
using Java.Util;
using MobileDbBenchmark.Droid;
using MobileDbBenchmark.UI;

[assembly: Xamarin.Forms.Dependency(typeof(DialogService))]

namespace MobileDbBenchmark.Droid
{
    public class DialogService : IDialogService
    {
        public void ShowProgressDialog()
        {
            UserDialogs.Instance.ShowLoading("Wykonuje test");

        }

        public void HideProgressDialog()
        {
            UserDialogs.Instance.HideLoading();
        }
    }
}