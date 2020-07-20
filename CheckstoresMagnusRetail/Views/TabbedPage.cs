using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using TabbedPagepo = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage;

namespace CheckstoresMagnusRetail.Views
{
    public class TabbedPages : Xamarin.Forms.TabbedPage
    {
        NavigationPage navigation;
        NavigationPage PerfilPage;
        NavigationPage navigationsecond;
        public TabbedPages()
        {
           Xamarin.Forms.NavigationPage.SetHasNavigationBar(this,false);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            navigation  = new NavigationPage(new HomeView());
            navigation.Title = "Inicio";
            navigation.IconImageSource = "home.png";
            navigation.Style =(Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];

             PerfilPage = new NavigationPage(new PerfilPageView());

            PerfilPage.Title = "Perfil";
            PerfilPage.IconImageSource = "user.png";
            PerfilPage.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];


            this.BackgroundColor = Color.FromHex("#F6F6F6");
            this.BarBackgroundColor = Color.White;
            this.SelectedTabColor = Color.FromHex("#763626");
            this.UnselectedTabColor = Color.FromHex("#eceff1");

        }

        protected override async void OnAppearing()
        {
            await Task.Delay(190);
            if (Children.Count == 0)
                cargadelayout();

            //NavigationPage navigation = new NavigationPage();

            //On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);



        }

        protected void cargadelayout() {


            Children.Add(navigation);
            Children.Add(PerfilPage);
        }
    }
}

