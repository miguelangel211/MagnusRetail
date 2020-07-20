using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicioViewCell : ViewCell
    {
        public static BindableProperty ServicioProperty = BindableProperty.Create(
      propertyName: "Servicio",
      returnType: typeof(Servicio),
      declaringType: typeof(ServicioViewCell),
      defaultValue: null,
      defaultBindingMode: BindingMode.TwoWay,
      propertyChanged: HandleservicepropertyChanged
      );

        public Servicio Servicio
        {
            get { return (Servicio)GetValue(ServicioProperty); }
            set { SetValue(ServicioProperty, value); }
        }

        private bool isbusy;

        public ServicioViewCell()
        {
            InitializeComponent();
            isbusy = false;
        }
        async void servicioclickedAsync(object sender, System.EventArgs e)
        {
            if (isbusy)
                return;
            // Debug.WriteLine("clicked");
            // await Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new TiendaDetailPage());
            //  Debug.WriteLine(Servicio.ServicioID.ToString());
            isbusy = true;
            var d = new NavigationPage(new TiendaDetailPage(Servicio));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            await App.Navigation.PushAsync(d);
            isbusy = false;
            // await Navigation.PushModalAsync(new NavigationPage(new TiendaDetailPage()));
        }


        private static void HandleservicepropertyChanged(BindableObject bindable, object oldavalue, object newvalue) {
            ServicioViewCell servcewll = (ServicioViewCell)bindable;
            Servicio serviciorecibido = ((Servicio)newvalue) ?? new Servicio();
            servcewll.ServicioName.Text = serviciorecibido.Nombre;
            servcewll.ServicioTipo.Text = serviciorecibido.TipoServicio;
            servcewll.ServicioID.Text ="("+ serviciorecibido.ServicioID.ToString()+")";
            if (serviciorecibido.ServicioEstatusID==1) {

                servcewll.EstatusServicio.BackgroundColor = Color.Green;
            }
   if(serviciorecibido.ServicioEstatusID == 2) {
                servcewll.EstatusServicio.BackgroundColor = Color.Red;
            }

            if (serviciorecibido.ServicioEstatusID == 3)
            {
                servcewll.EstatusServicio.BackgroundColor = (Color)Application.Current.Resources["mist"];
            }
            if (serviciorecibido.ServicioFechaHora.HasValue)
                servcewll.ServicioFecha.Text = (serviciorecibido.ServicioFechaHora.Value.ToShortDateString());
        }
    }

    
}
