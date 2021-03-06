﻿using MobileDbBenchamark.Common;
using Xamarin.Forms;

namespace MobileDbBenchmark.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var storageManager = DependencyService.Get<IStorageManager>();
            StorageManager.Instance = storageManager;
            MainPage = new MainPage();
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
