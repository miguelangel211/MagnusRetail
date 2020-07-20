using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.Helper;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using CheckstoresMagnusRetail.Views.ViewCells;
//using Plugin.Permissions;
using Xamarin.Forms;
//using ZXing.Mobile;
//using ZXing.Net.Mobile.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public partial class DetalleDeconteoPage : ContentPage
    {
         FormaDetalleConteoModel context;
        int apraciciones;
        Categoria categoria;
        TramoModel datos;
        public bool isbusy;
        public DetalleDeconteoPage(TramoModel parameter, Categoria categoriadelnivel)
        {
            isbusy = false;
            datos = parameter;
            categoria = categoriadelnivel;
            InitializeComponent();
            apraciciones = 0;
            Title = "MUEBLE " + parameter.Mueble.MueblePasillo + parameter.Mueble.MuebleCara;
            Nombretienda.Text = parameter.servicio.NombreTienda;
            Nombrecadena.Text = parameter.servicio.NombreCadena;
            nombremueble.Text = parameter.Tramo + " " + categoriadelnivel.CategoriaNombre;
            this.BindingContext = context = new FormaDetalleConteoModel(parameter, categoria);


/*
            MessagingCenter.Subscribe<AgregarProductoViewModel, string>(this, "Hi", async (sender, arg) =>
            {
                await Task.Delay(250);

                context.CargarData.Execute(null);

            });*/
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           // if (apraciciones == 0)
            //{
                await Task.Delay(200);
                context.CargarData.Execute(null);
                apraciciones++;
            //}
            /*
            MessagingCenter.Subscribe<ScannerViewModel, string>(this, "Hi", async (sender, arg) =>
            {
                if (isbusy)
                    return;
                isbusy = true;
                var d = new NavigationPage(new AgregarProductoPage(new ServicioMuebleProductoNivel
                { tramo = datos.Tramodata, producto = new Producto { UPC = arg } }, false, categoria));

                d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
                try
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Navigation.PushModalAsync(d);
                    });
                }
                catch { }
                isbusy = false;
            });*/

        }

        public async void openscanner(object sender, EventArgs args) {
            // var cameraPermission = new CameraPermission(CrossPermissions.Current);
            // await cameraPermission.RequestCameraPermissionIfNeeded();
            bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (allowed)
                await Navigation.PushAsync(new ScannerPageRB(datos, categoria));
            else await DisplayAlert("Alert", "You have to provide Camera permission", "Ok");

            /*
            // await Navigation.PushAsync((new ScannerPage(new ServicioMuebleProductoNivel { tramo=datos.Tramodata})));
            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = false,
                TryHarder = true,
          
            };
            var scanPage = new scanpagecustom()
                ;
            scanPage.Title = "Escanear Producto";
           
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {

                    scanPage.DefaultOverlayBottomText =result.Text;
                    var d = new NavigationPage(new AgregarProductoPage(new ServicioMuebleProductoNivel { tramo = datos.Tramodata, producto = new Producto { UPC = result.Text } }, false, categoria));

                    d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
                    try
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Navigation.PushModalAsync(d);
                        });
                    }
                    catch { }

                });
            };
            await Navigation.PushAsync(scanPage);
            */
        }

        ~DetalleDeconteoPage() {
         //   MessagingCenter.Unsubscribe<ScannerViewModel, string>(this, "Hi");

        }
        //  protected override bool OnBackButtonPressed()
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            //MessagingCenter.Unsubscribe<ScannerViewModel, string>(this, "Hi");

            return true;
        }
        
       /*
        {
            base.OnDisappearing();

        }*/

        public async void nuevoproducto(object sender, EventArgs args)
        {
            if (isbusy)
                return;
            isbusy = true;
            var d =
                //new NavigationPage
                (new AgregarProductoPage(new ServicioMuebleProductoNivel {tramo = datos.Tramodata },false, categoria));
                           // d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];

            await Navigation.PushAsync(d,false);
            isbusy = false;
        }

        async void tappedproducto(object sender, ItemTappedEventArgs e)
        {
            if (isbusy)
                return;
            isbusy = true;
            var d = //new NavigationPage
                (new AgregarProductoPage((e.Item as ServicioMuebleProductoNivel),true, categoria));

            //d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            await Navigation.PushAsync(d,false);
            isbusy = false;
        }
    }
}
