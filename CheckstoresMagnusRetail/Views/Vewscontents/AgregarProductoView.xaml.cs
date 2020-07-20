using System;
using System.Collections.Generic;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views.Vewscontents
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AgregarProductoView : ContentView
    {
        public AgregarProductoView()
        {
            InitializeComponent();
        }
        public async void openphotoviewer(object sender, EventArgs args)
        {
            var d = new NavigationPage(new ImagePhotoView(3, (this.BindingContext as AgregarProductoViewModel).DatosForma));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];

            await App.Navigation.PushModalAsync(d);
        }
    }
}
