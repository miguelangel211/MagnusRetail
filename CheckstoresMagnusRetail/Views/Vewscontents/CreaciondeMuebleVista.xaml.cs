using System;
using System.Collections.Generic;
using System.Diagnostics;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views.Vewscontents
{
    public partial class CreaciondeMuebleVista : ContentView
    {
        public CreaciondeMuebleVista()
        {
            InitializeComponent();
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }
        public async void openphotoviewer(object sender, EventArgs args)
        {
            var d = new NavigationPage(new ImagePhotoView(1, (this.BindingContext as CrearNuevoMuebleModel).MuebleData));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];

            await App.Navigation.PushModalAsync(d);
        }

        public async  void medidacamnbio(object sender, EventArgs e) {
            Debug.WriteLine((e as Syncfusion.XForms.ComboBox.ValueChangedEventArgs).Value);
            if ((e as Syncfusion.XForms.ComboBox.ValueChangedEventArgs).Value == "SI")
            {
                await (this.BindingContext as CrearNuevoMuebleModel).datocanbiadoenmedidas(true);
            }
            else {
                await (this.BindingContext as CrearNuevoMuebleModel).datocanbiadoenmedidas(false);

            }
        }
    }
}
