using Acr.UserDialogs;
using MobileDbBenchmark.UI;

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