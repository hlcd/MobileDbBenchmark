using MobileDbBenchamark.Common;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MobileDbBenchmark.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            // Get IStorageManager from DI at this point when MauiContext is available
            var storageManager = Handler?.MauiContext?.Services?.GetService(typeof(IStorageManager)) as IStorageManager;
            if (storageManager != null)
            {
                StorageManager.Instance = storageManager;
            }

            MainPage = new MainPage();

            return base.CreateWindow(activationState);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
