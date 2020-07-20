using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using CheckstoresMagnusRetail.Views.Vewscontents;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public partial class AgregarProductoPage : ContentPage
    {
        bool isbusy;
        AgregarProductoViewModel context;
        string upvalue;
        ServicioMuebleProductoNivel nivelelegido;
        int apariciones;
        bool Modoforma;
        public AgregarProductoPage(ServicioMuebleProductoNivel parameter,bool editando,Categoria paramcat)
        {
            nivelelegido = parameter;
            InitializeComponent();
            isbusy = false;
            Modoforma = editando;
            apariciones = 0;
            if(nivelelegido!=null)
                upvalue = (nivelelegido.producto?? new Producto()).UPC;
            this.Title = "AGREGAR PRODUCTO";
            this.BindingContext = context = new AgregarProductoViewModel(parameter,editando,paramcat);
           /* MessagingCenter.Subscribe<AgregarProductoViewModel, string>(this, "Hi", async (sender, arg) =>
            {
                context.CargarData.Execute(null);
            });*/
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (apariciones == 0)
            {
                await Task.Delay(500);
                this.Content = new AgregarProductoView();
                if (upvalue != null)
                    context.DatosForma.UPC = upvalue;
                context.CargarData.Execute(null);
                apariciones++;
            }
        }


      
        public async void close(object sender,EventArgs args) {
            try


            {
             //   this.Content.Opacity = .1;
                await Navigation.PopAsync();
            }
            catch { }
        }

        public async void guardardatos(object sender, EventArgs args) {
            // if (!Modoforma)
            // {
            if (isbusy)
                return;
            try
            {
                isbusy = true;
                this.Content = new GuardandoProducto();
                var p = await context.guardarproducto();
                if (p)
                {
                    await Navigation.PopAsync(false);

                }
                else
                {
                    this.Content = new AgregarProductoView();
                }
            }
            catch(Exception ex) {
                await context.escribirerror("Error al guardar datos : "+ex.Message,"Guardar datos producto","Modal Producto","",ex.StackTrace);
                this.Content = new AgregarProductoView();

            }
            isbusy = false;
            //}
           // else {
             //   await Navigation.PopModalAsync();

            //}

        }

        public async void openphotoviewer(object sender, EventArgs args)
        {
            var d = new NavigationPage(new ImagePhotoView(3,context.DatosForma));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];

            await Navigation.PushModalAsync(d);
        }
    }
}
