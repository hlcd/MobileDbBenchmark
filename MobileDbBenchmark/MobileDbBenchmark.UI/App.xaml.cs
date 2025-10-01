using MobileDbBenchamark.Common;
using Microsoft.Maui.Controls;

namespace MobileDbBenchmark.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#pragma warning disable CS0618 // Type or member is obsolete
            // Get from DI if available, otherwise fallback to DependencyService for compatibility
            var storageManager = Handler?.MauiContext?.Services?.GetService(typeof(IStorageManager)) as IStorageManager
                ?? Microsoft.Maui.Controls.DependencyService.Get<IStorageManager>();
            StorageManager.Instance = storageManager;
            MainPage = new MainPage();
#pragma warning restore CS0618 // Type or member is obsolete
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
