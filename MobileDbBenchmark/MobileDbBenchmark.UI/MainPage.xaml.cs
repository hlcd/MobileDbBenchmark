using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDbBenchmark.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainPageMenuItem;
            if (item == null)
                return;

            var page = Activator.CreateInstance(item.TargetType);
            var targetPage = page as MainPageDetail;

            if (targetPage != null)
            {
                targetPage.Init(item.Spec); 
                Detail = new NavigationPage(targetPage);
            }

            var notifications = page as NotificationsPage;
            if (notifications != null)
            {
                Detail = new NavigationPage(notifications);
            }
            
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}