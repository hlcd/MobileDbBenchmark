using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MobileDbBenchamark.Common;
using MobileDbBenchamark.Common.Models.Realm;
using MobileDbBenchamark.Common.Tests;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDbBenchmark.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPage : ContentPage
    {
        private readonly NotificationPageViewModel _viewModel;

        public NotificationsPage()
        {
            _viewModel = new NotificationPageViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
        }
    }

    public class NotificationPageViewModel : INotifyPropertyChanged
    {
        private readonly RealmBenchmark _realmBenchmark;
        private readonly DownloadService _downloadService;
        private int _remoteId = 1;
        public NotificationPageViewModel()
        {
            _realmBenchmark = new RealmBenchmark();
            LoadRealmPublicationCommand = new Command(async () =>
            {
                CurrentPublicationRealm = await _realmBenchmark.GetPublication(_remoteId);
                _remoteId++;
            });

            DownloadRealmPublicationCommand = new Command(async () => await DownloadRealmPublication());
            _downloadService = new DownloadService();
        }

        private async Task DownloadRealmPublication()
        {
            if (CurrentPublicationRealm == null)
                return;
            await _downloadService.DownloadRealmPublication(CurrentPublicationRealm.Id);
        }

        private Publication _currentPublicationRealm;

        public Publication CurrentPublicationRealm
        {
            get => _currentPublicationRealm;
            private set
            {
                _currentPublicationRealm = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadRealmPublicationCommand { get; }

        public ICommand DownloadRealmPublicationCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}