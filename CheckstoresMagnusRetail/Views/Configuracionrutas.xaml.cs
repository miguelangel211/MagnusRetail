using System;
using System.Collections.Generic;
using System.IO;
using CheckstoresMagnusRetail.ApiRepo;
using Plugin.FilePicker;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.Views
{
    public partial class Configuracionrutas : ContentPage
    {
        ApiRequest repo;
        public Configuracionrutas()
        {
            InitializeComponent();
            repo = new ApiRequest();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var rutabase=await  SecureStorage.GetAsync("rutadb");
            if (rutabase == "" || rutabase == null)
                rutabase = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Checkstorev2.db3") ;
            rutabd.Text = rutabase;

            var urlapibase = await SecureStorage.GetAsync("rutaapi");
            if (urlapibase == "" || urlapibase == null)
                urlapibase = " http://powerbinew.eastus.cloudapp.azure.com/magnusapi/";
            rutaapi.Text = urlapibase;

        }

        async void clickonbdchange(object senser,EventArgs args) {
            
                var file = await CrossFilePicker.Current.PickFile();

                if (file != null)
                {
                await SecureStorage.SetAsync("rutadb",file.FilePath);
                rutabd.Text = file.FilePath;
                await MaterialDialog.Instance.SnackbarAsync(message: "Ruta bd guardada: ",
            actionButtonText: "OK",
            msDuration: 2000);
            }



        }

        async void clickonchangerutaapi(object senser, EventArgs args)
        {


            
                await SecureStorage.SetAsync("rutaapi",rutaapi.Text.Trim() );
            await MaterialDialog.Instance.SnackbarAsync(message: "Ruta api guardada: ",
actionButtonText: "OK",
msDuration: 2000);

        }

       async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            probando.IsRunning = true;
           // await SecureStorage.SetAsync("rutaapi", rutaapi.Text.Trim());
            var rep = await repo.Pruebadeconexion2(rutaapi.Text.Trim());
            probando.IsRunning = false;

            await MaterialDialog.Instance.SnackbarAsync(message:rep.Errores,
                actionButtonText: "OK",
                msDuration: 2000);
        }

        void cerrarclicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
