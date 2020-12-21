using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MobileDbBenchamark.Common;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDbBenchmark.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageDetail : ContentPage
    {
        private readonly MainPageDetailViewModel _viewModel;

        public MainPageDetail()
        {
            _viewModel = new MainPageDetailViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
        }

        public void Init(TestSpec spec)
        {
            _viewModel.Init(spec);
        }

        public class MainPageDetailViewModel : INotifyPropertyChanged
        {
            private readonly IDialogService _dialogService;
            private readonly BenchmarkRunner _benchmarkRunner;
            private readonly IMemoryService _memoryService;

            public MainPageDetailViewModel()
            {
                RunRealmCommand = new Command(async () => await RunRealmTest());
                _dialogService = DependencyService.Get<IDialogService>();
                _memoryService = DependencyService.Get<IMemoryService>();
                _benchmarkRunner = new BenchmarkRunner();
                UpdateMemoryInfo();

            }


            private async Task RunRealmTest()
            {
                if (Spec == null)
                    return;

                Spec.DbType = DbType.Realm;
                await RunSpec(Spec);
            }

            private async Task RunSpec(TestSpec spec)
            {

                _dialogService.ShowProgressDialog();

                var results = await _benchmarkRunner.RunTest(spec);

                DisplayResults(spec, results);

                UpdateMemoryInfo();

                _dialogService.HideProgressDialog();
            }

            private void UpdateMemoryInfo()
            {
                NetMemory = GC.GetTotalMemory(false).ToString();
                NativeMemory = _memoryService.GetAllocatedMemory().ToString();
            }

            private void DisplayResults(TestSpec spec, List<TimeSpan> results)
            {
                var avg = results.Average(x => x.TotalMilliseconds).ToString("F");
                var min = results.Min(x => x.TotalMilliseconds).ToString("F");
                var max = results.Max(x => x.TotalMilliseconds).ToString("F");
                RealmAvg = avg;
                RealmBest = min;
                RealmWorst = max;

            }

            private string _realmWorst;
            public string RealmWorst
            {
                get => _realmWorst;
                set
                {
                    _realmWorst = value;
                    OnPropertyChanged();
                }
            }

            private string _realmBest;
            public string RealmBest
            {
                get => _realmBest;
                set
                {
                    _realmBest = value;
                    OnPropertyChanged();
                }
            }

            private string _realmAvg;
            public string RealmAvg
            {
                get => _realmAvg;
                set
                {
                    _realmAvg = value;
                    OnPropertyChanged();
                }
            }

            public string TestName => Spec == null ? "Wybierz test" : Spec.Name;


            private TestSpec _spec;

            public TestSpec Spec
            {
                get => _spec;
                set
                {
                    _spec = value;
                    OnPropertyChanged();
                }
            }


            public bool CanRunTest => Spec != null;

            public ICommand RunRealmCommand { get; }

            public ICommand RunSqliteCommand { get; }

            private string _netMemory;

            public string NetMemory
            {
                get { return _netMemory; }
                set
                {
                    _netMemory = value;
                    OnPropertyChanged();
                }
            }

            private string _nativeMemory;

            public string NativeMemory
            {
                get { return _nativeMemory; }
                set
                {
                    _nativeMemory = value;
                    OnPropertyChanged();
                }
            }


            public void Init(TestSpec spec)
            {
                Spec = spec;
                RealmAvg = string.Empty;
                RealmBest = string.Empty;
                RealmWorst = string.Empty;

                OnPropertyChanged(nameof(CanRunTest));

            }



            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion


        }
    }
}