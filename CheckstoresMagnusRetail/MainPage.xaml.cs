using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        ApiRequest api = new ApiRequest();
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing() {
            base.OnAppearing();
            await loginUsuario();
        }

        public async Task loginUsuario() {
            var color = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration();
            color.BackgroundColor= Color.FromHex("#336B87");
            var resultado =await  api.LoginUsuario("admin","admin");
            if (resultado.realizado)
            {
                if (resultado.Result.Autenticado)
                {
                   await Navigation.PushAsync(new TabbedPages());
                }
                else {
                    await MaterialDialog.Instance.SnackbarAsync(message: resultado.Errores,
                            actionButtonText: "OK",
                            msDuration: 3000,color);
                }
            }
            else {
                await MaterialDialog.Instance.SnackbarAsync(message: "No se pudo establecer la conexion",
                                            actionButtonText: "OK",
                                            msDuration: 3000,color);
            }
           // new NavigationPage(new Views.LoginPage())
        }
    }
}
