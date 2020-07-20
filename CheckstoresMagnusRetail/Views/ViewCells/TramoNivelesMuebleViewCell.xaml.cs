using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TramoNivelesMuebleViewCell : ViewCell
    {
        public static BindableProperty TramoFormaMueblesModelProperty = BindableProperty.Create(
        propertyName: "TramoFormaMueblesModel",
        returnType: typeof(TramoFormaMueblesModel),
        declaringType: typeof(TramoNivelesMuebleViewCell),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: HandleservicepropertyChanged
        );

        public static ObservableCollection<Nivel> niveles = new ObservableCollection<Nivel>();
        public static ObservableCollection<Nivel> Niveles
        {
            get { return niveles; }
            set { niveles = value; }
        }

        public TramoFormaMueblesModel TramoFormaMueblesModel
        {
            get { return (TramoFormaMueblesModel)GetValue(TramoFormaMueblesModelProperty); }
            set { SetValue(TramoFormaMueblesModelProperty, value); }
        }
        public TramoNivelesMuebleViewCell()
        {
            InitializeComponent();

            //nombretramo.Text = "Tramo 1";
            //listView.ItemsSource = new List<Nivel> { new Nivel {NombreNivel="NIVEL 1" },new Nivel {NombreNivel="NIVEL 2" },new Nivel {NombreNivel="NIVEL 3" } };

        }

        private static void HandleservicepropertyChanged(BindableObject bindable, object oldavalue, object newvalue)
        {
            TramoNivelesMuebleViewCell tramocell = (TramoNivelesMuebleViewCell)bindable;
            TramoFormaMueblesModel tramorecibido = ((TramoFormaMueblesModel)newvalue) ;
            Debug.WriteLine("aqui");
            //tramocell.nombretramo.Text = "Tramo 1";
           // Niveles = tramorecibido.TramoName;
           // tramocell.listView.ItemsSource = tramorecibido.Niveles;

        }
    }
}
