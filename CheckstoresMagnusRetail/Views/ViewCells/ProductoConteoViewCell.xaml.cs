using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductoConteoViewCell : ViewCell
    {
        public static BindableProperty ProductoProperty = BindableProperty.Create(
              propertyName: "Producto",
              returnType: typeof(ServicioMuebleProductoNivel),
              declaringType: typeof(ProductoConteoViewCell),
              defaultValue: null,
              defaultBindingMode: BindingMode.OneWay,
              propertyChanged: HandleservicepropertyChanged
              );

        public ServicioMuebleProductoNivel Producto
        {

            get { return (ServicioMuebleProductoNivel)GetValue(ProductoProperty); }
            set { SetValue(ProductoProperty, value); }
        }

        public ProductoConteoViewCell()
        {
            InitializeComponent();
        }

        private static void HandleservicepropertyChanged(BindableObject bindable, object oldavalue, object newvalue)
        {
            ProductoConteoViewCell productocell = (ProductoConteoViewCell)bindable;
            ServicioMuebleProductoNivel prodrecibido = ((ServicioMuebleProductoNivel)newvalue) ?? new ServicioMuebleProductoNivel();
            try
            {
                productocell.Nombre.Text = string.Concat("NOMBRE: ", prodrecibido.producto.Nombre);
                productocell.Marca.Text = string.Concat("MARCA: ", prodrecibido.producto.Marca);
                productocell.Fabricante.Text = prodrecibido.producto.Fabricante;
               // productocell.Cantfrente.Text = string.Concat("CANT FRENTE: ", prodrecibido.Frente);
                productocell.Gramaje.Text = string.Concat("GRAMAJE: ", prodrecibido.producto.Gramaje);
                productocell.Alto.Text = String.Concat("ALTO: ", prodrecibido.producto.Alto);
                productocell.Ancho.Text = string.Concat("ANCHO: ", prodrecibido.producto.Ancho);
                productocell.Produndo.Text = string.Concat("PROFUNDO: ", prodrecibido.producto.Profundo);
                productocell.UPC.Text = string.Concat("UPC: ",prodrecibido.producto.UPC);
              //  productocell.Cantprofundo.Text = string.Concat("CANT PROFUNDO: ", prodrecibido.Profundo);
            }
            catch { }

        }
    }
}
