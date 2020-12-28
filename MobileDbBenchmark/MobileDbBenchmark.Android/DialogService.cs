using MobileDbBenchmark.Droid;
using MobileDbBenchmark.UI;

[assembly: Xamarin.Forms.Dependency(typeof(DialogService))]

namespace MobileDbBenchmark.Droid
{
    public class DialogService : IDialogService
    {
        public void ShowProgressDialog()
        {
        }

        public void HideProgressDialog()
        {
        }
    }
}