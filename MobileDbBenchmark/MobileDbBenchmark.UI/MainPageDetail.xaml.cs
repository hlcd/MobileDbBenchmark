﻿using System;
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

            public MainPageDetailViewModel()
            {
                RunRealmCommand = new Command(async () => await RunRealmTest());
                RunSqliteCommand = new Command(async () => await RunSqliteTest());
                _dialogService = DependencyService.Get<IDialogService>();
                _benchmarkRunner = new BenchmarkRunner();
            }

            private async Task RunSqliteTest()
            {
                if (Spec == null)
                    return;

                Spec.DbType = DbType.Sqlite;
                await RunSpec(Spec);
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

                _dialogService.HideProgressDialog();
            }

            private void DisplayResults(TestSpec spec, List<TimeSpan> results)
            {
                var avg = results.Average(x => x.TotalMilliseconds).ToString("F");
                var min = results.Min(x => x.TotalMilliseconds).ToString("F");
                var max = results.Max(x => x.TotalMilliseconds).ToString("F");
                if (spec.DbType == DbType.Sqlite)
                {
                    SqliteAvg = avg;
                    SqliteBest = min;
                    SqliteWorst = max;
                }
                else
                {
                    RealmAvg = avg;
                    RealmBest = min;
                    RealmWorst = max;
                }
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

            private string _sqliteWorst;
            public string SqliteWorst
            {
                get => _sqliteWorst;
                set
                {
                    _sqliteWorst = value;
                    OnPropertyChanged();
                }
            }

            private string _sqliteBest;
            public string SqliteBest
            {
                get => _sqliteBest;
                set
                {
                    _sqliteBest = value;
                    OnPropertyChanged();
                }
            }

            private string _sqliteAvg;
            public string SqliteAvg
            {
                get => _sqliteAvg;
                set
                {
                    _sqliteAvg = value;
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

            public void Init(TestSpec spec)
            {
                Spec = spec;

                SqliteAvg = string.Empty;
                SqliteBest = string.Empty;
                SqliteWorst = string.Empty;
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