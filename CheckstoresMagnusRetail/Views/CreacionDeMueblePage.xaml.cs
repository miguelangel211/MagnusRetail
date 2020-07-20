using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.ViewModels;
using CheckstoresMagnusRetail.Views.Vewscontents;
using CheckstoresMagnusRetail.Views.ViewCells;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views
{
    public partial class CreacionDeMueblePage : ContentPage
    {
        CrearNuevoMuebleModel context;
        CreaciondeMuebleVista vista;
        int apariciones;
        bool Editando;
        public CreacionDeMueblePage(MuebleModel mueble,bool editando)
        {
            InitializeComponent();
            apariciones = 0;
            Editando = editando;
            this.BindingContext = context = new CrearNuevoMuebleModel(editando,mueble);
            if (!editando)
            {
                Title = "Agregar Mueble";
            }
            else {
                Title = mueble.Mueble;
            }
        }

        protected override async void OnAppearing() {
            base.OnAppearing();

            if (apariciones==0) {
                await Task.Delay(1000);
                vista = new CreaciondeMuebleVista();
                context.CargarData.Execute(null);
                apariciones++;
                this.Content = vista;

            }
        }


        public async void openphotoviewer(object sender, EventArgs args)
        {
            var d = new NavigationPage(new ImagePhotoView(1,context.MuebleData));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];

            await Navigation.PushModalAsync(d);
        }
        public async void close(object sender, EventArgs args)
        {
            try
            {
                await App.Navigation.PopAsync();

            }
            catch { }
          
        }
        public async void Aceptarcambio(object sender,EventArgs args) {
            if (Editando)
            {
                close(sender, args);
                return;
            }
            if (await context.Verificardatacompleta())
            {
                this.Content = new GuardandoMuebleView();
                var guardado = await context.AlmacenarDatos();
                if (guardado)
                {
                    close(sender, args);
                }
                else
                {

                }
            }
        }


    }
}
