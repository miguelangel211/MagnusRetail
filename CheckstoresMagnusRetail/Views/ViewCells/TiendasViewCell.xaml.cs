using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TiendasViewCell : ViewCell
    {
        public static BindableProperty TiendaProperty = BindableProperty.Create(
            propertyName:"Tienda",
            returnType:typeof(TiendaDataModel),
            declaringType:typeof(TiendasViewCell),
            defaultValue:null,
            defaultBindingMode:BindingMode.OneWay,
            propertyChanged:Handlestorepropertychanged
            );

        public Command esconder;
        public Command Esconder { get { return esconder; } set { esconder = value; } }
        static string link = "https://www.google.com/maps/place/Smart+And+Final+Centro/@32.5355237,-117.0305324,18.12z/data=!4m5!3m4!1s0x0:0x850256c585e60940!8m2!3d32.5363488!4d-117.0315143";
        public TiendaDataModel Tienda {
            get { return (TiendaDataModel)GetValue(TiendaProperty); }
            set { SetValue(TiendaProperty, value); }
        }


        public TiendasViewCell()
        {
            InitializeComponent();
        }

        private static void Handlestorepropertychanged(BindableObject bindable,object oldvalue,object newvalue) {
            TiendasViewCell cell = (TiendasViewCell)bindable;
            TiendaDataModel tiendarecibida = ((TiendaDataModel)newvalue)?? new TiendaDataModel();
            cell.ChainName.Text = tiendarecibida.NombreCadena;
            cell.TiendaName.Text = tiendarecibida.NombreTienda;
            link = tiendarecibida.Ubicacion;
           

        }
        public void linkclicked(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri(Tienda.Ubicacion));
        }

        public void tiendaclicked(object sender,System.EventArgs e) {

            if ((this.BindingContext as TiendaModel).Expanded == true)
            {

                (this.BindingContext as TiendaModel).Expanded = false;

                (this.BindingContext as TiendaModel).Clear();
            }
            else
            {

                (this.BindingContext as TiendaModel).Expanded = true;

                (this.BindingContext as TiendaModel).expandir();
            }
        }

     
    }
}
