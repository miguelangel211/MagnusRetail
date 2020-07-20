using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TramoViewCell : ViewCell
    {
        public static BindableProperty TramoProperty = BindableProperty.Create(
            propertyName: "Tramo",
            returnType: typeof(TramoModel),
            declaringType: typeof(TramoViewCell),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: HandleservicepropertyChanged
            );
        public bool Isbusy;
        public static ObservableCollection<Categoria> productos = new ObservableCollection<Categoria>();
        public static ObservableCollection<Categoria> Productos {
            get { return productos; }
            set { productos = value; }
        }
        public TramoModel Tramo
        {

            get { return (TramoModel)GetValue(TramoProperty); }
            set { SetValue(TramoProperty, value);Debug.WriteLine("cuanto"); }
        }

        public static ProductosContext context;

        public TramoViewCell()
        {
            InitializeComponent();
            ListProducto.ItemsSource = Productos;
            Isbusy = false;
        // ListProducto.ItemTapped += (object sender, ItemTappedEventArgs e) => { Debug.WriteLine("Sleccionado producto"); };
        }

        private static void HandleservicepropertyChanged(BindableObject bindable, object oldavalue, object newvalue)
        {
            TramoViewCell tramocell = (TramoViewCell)bindable;
            TramoModel tramorecibido = ((TramoModel)newvalue) ?? new TramoModel();
            tramocell.TramoName.Text = tramorecibido.Tramo;
            tramocell.ListProducto.ItemsSource = tramorecibido.productos ?? new ObservableCollection<Categoria>();
            tramocell.ListProducto.HeightRequest = 20 * (tramorecibido.productos ?? new ObservableCollection<Categoria>()).Count() +5;
        }

        public async void productoseleccionado(object sender,Syncfusion.ListView.XForms.ItemTappedEventArgs args) {
            //  var d = new NavigationPage(new DetalleDeconteoPage());

            // d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            if (Isbusy)
                return;
            Isbusy = true;
           await App.Navigation.PushAsync(new DetalleDeconteoPage(Tramo,(Categoria)args.ItemData));
            Isbusy = false;
        }



    }
}
