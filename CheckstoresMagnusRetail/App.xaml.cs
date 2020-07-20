using System;
using CheckstoresMagnusRetail.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail
{
    public partial class App : Application
    {
        public static INavigation Navigation { get; set; }


        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjI0NjA2QDMxMzcyZTM0MmUzMFJ4ZVhWNEdmRktITlRHSUhMRkpvUjcvSnFvOHltUnY5K25NMVRkTFdwMm89;MjI0NjA3QDMxMzcyZTM0MmUzMEMzdzJTUU52ZlZNQ2h4MDRsSzdFdVlRTVNjVkx1K0l4YkVncVVBeWNtZ009");
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            var d = new NavigationPage(new LoginPage());
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            MainPage = d;
            
            Navigation = MainPage.Navigation;
          
            
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
