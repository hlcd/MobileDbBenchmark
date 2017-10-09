using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MobileDbBenchamark.Common;
using MobileDbBenchamark.Common.Models.Realm;
using MobileDbBenchamark.Common.Tests;
using MobileDbBenchmark.UI.Annotations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SqlPublication = MobileDbBenchamark.Common.Models.Sqlite.Publication;

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
        private readonly SqliteBenchmark _sqliteBenchmark;
        private int _remoteId = 1;
        private int _sqliteRemoteId = 1;
        public NotificationPageViewModel()
        {
            _realmBenchmark = new RealmBenchmark();  
            _sqliteBenchmark = new SqliteBenchmark();
            LoadRealmPublicationCommand = new Command(async () =>
            {
                CurrentPublicationRealm = await _realmBenchmark.GetPublication(_remoteId);
                _remoteId++;
            });

            DownloadRealmPublicationCommand = new Command(async () => await DownloadRealmPublication());

            LoadSqlitePublicationCommand = new Command(async () =>
            {
                var data =  await _sqliteBenchmark.GetPublication(_sqliteRemoteId);
                CurrentPublicationSqlite = new SqlitePublication(data.Id);
                CurrentPublicationSqlite.DownloadPercentage = data.DownloadPercentage;
                _sqliteRemoteId++;
            });

            DownloadSqlitePublicationCommand = new Command(async () => await DownloadSqlitePublication());
            _downloadService = new DownloadService();
        }

        private async Task DownloadSqlitePublication()
        {
            if (CurrentPublicationSqlite == null)
                return;
            _downloadService.OnPublicationDownloadProgressChanged += DownloadServiceOnOnPublicationDownloadProgressChanged;
            await _downloadService.DownloadSqlitePublication(CurrentPublicationSqlite.Id);
            _downloadService.OnPublicationDownloadProgressChanged -= DownloadServiceOnOnPublicationDownloadProgressChanged;
        }

        private void DownloadServiceOnOnPublicationDownloadProgressChanged(DownloadPubblicationEventArgs args)
        {
            if(CurrentPublicationSqlite == null)
                return;

            if (args.Id == CurrentPublicationSqlite.Id)
            {
                CurrentPublicationSqlite.DownloadPercentage = args.DownloadPercentage;
                //or load all data from db
            }
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

        private SqlitePublication _currentPublicationSqlite;

        public SqlitePublication CurrentPublicationSqlite
        {
            get => _currentPublicationSqlite;
            private set
            {
                _currentPublicationSqlite = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadSqlitePublicationCommand { get; }

        public ICommand DownloadSqlitePublicationCommand { get; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class SqlitePublication : INotifyPropertyChanged
        {
            public SqlitePublication(string id)
            {
                Id = id;
            }


            public string Id { get; }

            private int _downloadPercentage;

            public int DownloadPercentage
            {
                get => _downloadPercentage;
                set
                {
                    _downloadPercentage = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    
}