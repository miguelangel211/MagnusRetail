using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using FFImageLoading;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public partial class ImagePhotoView : ContentPage
    {
        public ObservableCollection<Stream> ListadeImagenes { get; set; }
        bool isbusy;
        MediaFile photo;
        PhotoGalleryModel context;
        public ImagePhotoView(int tipogaleria,object origen)
        {
            InitializeComponent();
            this.Title = "Fotos";
           this.BindingContext= context = new PhotoGalleryModel(tipogaleria, origen);
        }

         void Modoseleccionado(object sender, EventArgs args) {
            context.TomarFoto.Execute(null);
        }
        public async void close(object sender, EventArgs args)
        {
            if (isbusy)
                return;
            isbusy = true;
            try
            {
                await Navigation.PopModalAsync(false);
            }
            catch { }
            isbusy = false;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
           await Task.Delay(250);
            context.CargarData.Execute(null);


        }

        async void tomarfotoclick(object sender,EventArgs args) {
            //  fotosource.Focus();
            context.TomarFoto.Execute(null);


        }

        async void eliminarfoto(object sender, EventArgs args)
        {
            //  fotosource.Focus();
            context.EliminarFoto();

        }

        async void fotoselccionada(object sender, EventArgs args)
        {
            var sen = (sender as Syncfusion.ListView.XForms.SfListView);
           if(sen.CurrentItem!=null)
                context.FotoActual=   (sen.CurrentItem as photoobject).ContactImage;
            //  fotosource.Focus();
            //Debug.WriteLine("Aqui");

        }
    }
}
