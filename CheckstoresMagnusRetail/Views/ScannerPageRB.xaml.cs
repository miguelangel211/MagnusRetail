using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using GoogleVisionBarCodeScanner;
//using Rb.Forms.Barcode.Pcl;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace CheckstoresMagnusRetail.Views
{
    public partial class ScannerPageRB : ContentPage
    {
        ScannerViewModel context;
        TramoModel Tramo;
        Categoria Categorias;
        public ScannerPageRB( TramoModel tramo,Categoria categoria)
        {
          //  this.BindingContext = context = new ScannerViewModel(tramo,categoria);
            
            InitializeComponent();
            //GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(GoogleVisionBarCodeScanner.BarcodeFormats.Upca);

            Tramo = tramo;
            Categorias = categoria;
            //  NavigationPage.SetHasBackButton(this, false);
            this.Title = "Escaner de productos";

            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            /**
             * So that we can release the camera when turning off phone or switching apps.
             */
            // MessagingCenter.Subscribe<App>(this, App.MessageOnSleep, disableScanner);
            // MessagingCenter.Subscribe<App>(this, App.MessageOnResume, enableScanner);
            //  barcodeScanneri.BarcodeChanged += animateFlash;
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();

            //  await Task.Delay(200);
            //barcodeScanneri.IsEnabled = true;
            //  context.Activo = true;
            // context.Decoder = true;
            //  GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            //  GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);

        }



        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            List<GoogleVisionBarCodeScanner.BarcodeResult> obj = e.BarcodeResults;

            string result = string.Empty;
            result = obj[0].DisplayValue;
            //GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

            var d =
                //new NavigationPage
                (new AgregarProductoPage(new ServicioMuebleProductoNivel
                {
                    tramo = Tramo.Tramodata,
                    producto = new Producto { UPC = result }
                }, false, Categorias));

              //  d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
                try
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Navigation.PushAsync(d,false);
                    });
                }
                catch { }


            IsBusy = false;

        }

        protected override void OnDisappearing()
        {
            //barcodeScanneri.IsEnabled = false;
            base.OnDisappearing();
        }

        public void DisableScanner()
        {
            disableScanner(null);
        }

        public async void prenderfoco(object sender,EventArgs args) {
            // Turn On
            /*
            if (context.Torch)
            {
                barcodeScanneri.Torch = true;

            }
            else
            {
                // Turn Off
                barcodeScanneri.Torch = false;
            }*/
            GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                disableScanner(this);
             //   barcodeScanneri.BarcodeChanged -= animateFlash;
            }
            catch { }
            return base.OnBackButtonPressed();
        }

        private void gotoThirdPage(Object sender, EventArgs e)
        {
        //    Navigation.PushAsync(new prod());
        }


        /**
         * Release camera so that other apps can access it.
         */
        private void disableScanner(object sender)
        {
         //   barcodeScanneri.IsEnabled = false;
        }

        /**
         * All your camera belongs to us.
         */
        private void enableScanner(object sender)
        {
           // barcodeScanneri.IsEnabled = true;
        }

     /*   private async void animateFlash(object sender, BarcodeEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                await flash.FadeTo(1, 150, Easing.CubicInOut);
                flash.Opacity = 0;
            });
        }*/

        /**
         * You need to take care of realeasing the camera when you are done with it else bad things can happen!
         */
        ~ScannerPageRB()
        {
            try
            {
                disableScanner(this);

                /**
                 * Camera is released we dont need the events anymore.
                 */
                //  MessagingCenter.Unsubscribe<App>(this, App.MessageOnSleep);
                //MessagingCenter.Unsubscribe<App>(this, App.MessageOnResume);

            //    barcodeScanneri.BarcodeChanged -= animateFlash;
            }
            catch { }
        }
    }
}
