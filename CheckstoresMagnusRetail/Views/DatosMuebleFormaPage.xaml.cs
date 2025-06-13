using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public partial class DatosMuebleFormaPage : ContentPage
    {

        
        public DatosMuebleFormaPage()
        {
            InitializeComponent();
        }

        public async void CrearNuevoMueble(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new ImagePhotoView());
        }


        public async void Salir(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }
    }
}
