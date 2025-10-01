using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using MobileDbBenchamark.Common;
using MobileDbBenchmark.Droid;
using MobileDbBenchmark.UI;

namespace MobileDbBenchmark
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<UI.App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register platform services
            builder.Services.AddSingleton<IStorageManager, AndroidStorageManager>();
            builder.Services.AddSingleton<IMemoryService, MemoryService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            // Configure Acr.UserDialogs
#if ANDROID
            Acr.UserDialogs.UserDialogs.Init(() => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity);
#endif

            return builder.Build();
        }
    }
}