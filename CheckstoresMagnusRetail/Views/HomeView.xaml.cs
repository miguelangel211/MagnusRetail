using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        HomeViewModel viewmod;
        public HomeView()
        {
            InitializeComponent();
           
            BindingContext = viewmod= new HomeViewModel();
            var currentVersion = VersionTracking.CurrentVersion;
            this.Title = "Check Stores  V "+currentVersion;
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) => tappedlog();
            this.loglayout.GestureRecognizers.Add(tgr);
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(200); //wait ui to load
                (this.BindingContext as HomeViewModel).CargarData.Execute(null);
        }

        async void servicioclickedAsync(object sender, System.EventArgs e)
        {
            SQLitemethods db = new SQLitemethods();
             db.createdb();
             await viewmod.recargarDatos();

        }

        
        async void cerrarlog(object sender, System.EventArgs e)
        {
 
             viewmod.Logginvisible=false;

        }


        async void tappedlog()
        {
            await App.Navigation.PushAsync(new LogPage(viewmod.LOGtEXT));
        }
        async void tappedservice(object sender, ItemTappedEventArgs e) {
            if (!viewmod.ListaHabilitada)
                return;
            await App.Navigation.PushAsync(new TiendaDetailPage((Servicio)e.Item));
        }
    }
}
