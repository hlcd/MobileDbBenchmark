using System.ComponentModel;
using System.Runtime.CompilerServices;
using MobileDbBenchamark.Common.Models.Realm;
using MobileDbBenchmark.UI.Annotations;
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
        private Publication _currentPublication;

        public Publication CurrentPublication
        {
            get { return _currentPublication; }
            private set
            {
                _currentPublication = value;
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