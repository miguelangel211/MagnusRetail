using System;
using System.Collections.Generic;
using System.Diagnostics;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    
    public partial class Servicios : ViewCell
    {
        HomeViewModel viewmod;

        public Servicios()
        {
            InitializeComponent();
           // this.BindingContext =viewmod =new HomeViewModel(Navigation);

        }

        async void servicioclickedAsync(object sender, System.EventArgs e)
        {
            Debug.WriteLine("clicked");
            //await Navigation.PushModalAsync(new NavigationPage(new TiendaDetailPage()));
        }

  

    }
}
