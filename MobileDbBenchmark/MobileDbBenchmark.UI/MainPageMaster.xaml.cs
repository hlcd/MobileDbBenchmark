using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MobileDbBenchamark.Common;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDbBenchmark.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageMaster : ContentPage
    {
        public ListView ListView;

        public MainPageMaster()
        {
            InitializeComponent();

            BindingContext = new MainPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MainPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainPageMenuItem> MenuItems { get; set; }
            private IList<TestSpec> Specs { get; }
            public MainPageMasterViewModel()
            {
                Specs = new List<TestSpec>
                {
                    new TestSpec
                    {
                        Name = "Insert",
                        NumberOfItems = 10000,
                        RemoveDbBetweenIterations = true,
                        RepeatTimes = 5,
                        TestCase = BenchmarkTest.Insert
                    },
                    new TestSpec
                    {
                        Name = "Insert2",
                        NumberOfItems = 10000,
                        RemoveDbBetweenIterations = false,
                        RepeatTimes = 5,
                        TestCase = BenchmarkTest.Insert
                    },
                    new TestSpec
                    {
                        Name = "Count",
                        NumberOfItems = 10000,
                        RemoveDbBetweenIterations = true,
                        RepeatTimes = 5,
                        TestCase = BenchmarkTest.Count
                    },
                    new TestSpec
                    {
                        Name = "Select",
                        NumberOfItems = 10000,
                        RemoveDbBetweenIterations = true,
                        RepeatTimes = 5,
                        TestCase = BenchmarkTest.Select
                    },
                    new TestSpec
                    {
                    Name = "Update",
                    NumberOfItems = 10000,
                    RemoveDbBetweenIterations = true,
                    RepeatTimes = 5,
                    TestCase = BenchmarkTest.Update
                    },
                };

                MenuItems = new ObservableCollection<MainPageMenuItem>(Specs.Zip(Enumerable.Range(0, 100), (spec, i) => new MainPageMenuItem
                {
                    Id = i,
                    Spec = spec,
                    Title = spec.Name
                }));
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