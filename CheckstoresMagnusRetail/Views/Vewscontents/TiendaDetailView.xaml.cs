using System;
using System.Collections.Generic;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.Views.Vewscontents
{
    public partial class TiendaDetailView : ContentView
    {
        XF.Material.Forms.UI.Dialogs.Configurations.MaterialAlertDialogConfiguration segundocolor = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialAlertDialogConfiguration();
        ServiciosOperaciones serepo = new ServiciosOperaciones();

        public TiendaDetailView(int? Estatus)
        {
            InitializeComponent();
            segundocolor.BackgroundColor = (Color)Application.Current.Resources["azul"];
            segundocolor.MessageTextColor = Color.White;
            segundocolor.TitleTextColor = (Color)Application.Current.Resources["mist"];
            segundocolor.TintColor = (Color)Application.Current.Resources["mist"];
            this.concluirbutton.DisabledBackgroundColor = (Color)Application.Current.Resources["mist"];
            if (Estatus == 3) {
                this.concluirbutton.IsEnabled = false;
                this.concluirbutton.Text = "Servicio concluido";
            }


        }
        public async void openphotoviewer(object sender, EventArgs args)
        {
            var d = new NavigationPage(new ImagePhotoView(2,(this.BindingContext as TiendaViewModel).servicioactual));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            await App.Navigation.PushModalAsync(d);
        }

        public void tiendaclicked(object sender, System.EventArgs e)
        {

            (this.BindingContext as TiendaModel).Expanded = false;
        }
        public async void Concluirservicioclick(object sender, EventArgs args){
            try
            {
                (this.BindingContext as TiendaViewModel).Eliminando = true;

                bool? respuesta = await MaterialDialog.Instance.ConfirmAsync(message: "concluir este servicio",
                         title: "Confirmar",
                         confirmingText: "SI",
                         dismissiveText: "NO", segundocolor);
                if (respuesta ?? false)
                {


                    var servicio = (this.BindingContext as TiendaViewModel).servicioactual;
                    await serepo.ConcluirServicio(servicio);
                   //this.concluirbutton.Text = "Servicio concluido";
                    this.concluirbutton.IsEnabled = false;
                    (this.BindingContext as TiendaViewModel).Eliminando = false;

                }
            }
            catch(Exception ex) 
            {
            
            }

        }
    }
}
