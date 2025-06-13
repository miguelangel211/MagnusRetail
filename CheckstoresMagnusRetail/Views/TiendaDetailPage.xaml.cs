using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using CheckstoresMagnusRetail.Views.Vewscontents;
using CheckstoresMagnusRetail.Views.ViewCells;
using Xamarin.Forms;
using XF.Material.Forms.UI;

namespace CheckstoresMagnusRetail.Views
{
    public partial class TiendaDetailPage : ContentPage
    {
        TiendaViewModel context;
        int apariciones;
        private bool isbusy;
        public TiendaDetailPage(Servicio service)
        {
            InitializeComponent();
            this.BindingContext = context = new TiendaViewModel(service);
            isbusy = false;
            apariciones = 0;
            this.Title = service.NombreTienda;
            
            MessagingCenter.Subscribe<MuebleViewcell, MuebleModel>(this, "Hi", async (sender, arg) =>
            {
              await  context.EliminarMueble(arg);
            });
            MessagingCenter.Subscribe<CrearNuevoMuebleModel, string>(this, "Hi", async (sender, arg) =>
            {
                 context.CargarData.Execute(null);
            });
        }

        protected override bool OnBackButtonPressed()
        {
             base.OnBackButtonPressed();
            if (context.Eliminando)
            {
                return true;
            }
            else {
                return false;
            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (apariciones == 0)
            {
                if (context.MueblesEnTienda.Count == 0)
                {
                    await Task.Delay(500);
                    this.Content = new TiendaDetailView(context.servicioactual.ServicioEstatusID
    );
                    context.CargarData.Execute(null);
                    apariciones++;

                }
            }





        }




        public async void CrearNuevoMueble(object sender, EventArgs args)
        {
            if (isbusy)
                return;
            isbusy = true;
            var d = new NavigationPage(new CreacionDeMueblePage(new DataModels.MuebleModel
            {ServicioID=context.servicioactual.ServicioID },false));
           
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            await Navigation.PushAsync(d);
            isbusy = false;
        }





        public async void Salir(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

     

    }
}
