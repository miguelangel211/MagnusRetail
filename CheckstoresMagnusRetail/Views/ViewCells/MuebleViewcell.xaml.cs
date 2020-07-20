using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MuebleViewcell : ViewCell
    {
        public static BindableProperty MuebleProperty = BindableProperty.Create(
            propertyName: "Mueble",
            returnType: typeof(MuebleModel),
            declaringType: typeof(MuebleViewcell),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: HandleservicepropertyChanged
            );

        public MuebleModel Mueble {

            get { return (MuebleModel)GetValue(MuebleProperty); }
            set { SetValue(MuebleProperty, value); }
        }

        XF.Material.Forms.UI.Dialogs.Configurations.MaterialAlertDialogConfiguration segundocolor = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialAlertDialogConfiguration();

        public static readonly BindableProperty ParentContextProperty =
           BindableProperty.Create("ParentContext", typeof(object), typeof(MuebleViewcell), null, propertyChanged: OnParentContextPropertyChanged);

        public object ParentContext
        {
            get { return GetValue(ParentContextProperty); }
            set { SetValue(ParentContextProperty, value); }
        }

        private static void OnParentContextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue && newValue != null)
            {
                (bindable as MuebleViewcell).ParentContext = newValue;
            }
        }


        public MuebleViewcell()
        {
            InitializeComponent();
            segundocolor.BackgroundColor = Color.Red;
            segundocolor.MessageTextColor = Color.White;
            segundocolor.TitleTextColor = Color.White;
            segundocolor.TintColor = Color.White;
        }

        private static void HandleservicepropertyChanged(BindableObject bindable, object oldavalue, object newvalue)
        {
            MuebleViewcell mueblecell = (MuebleViewcell)bindable;
            MuebleModel mueblerecibido = ((MuebleModel)newvalue) ?? new MuebleModel();
            mueblecell.MuebleName.Text = mueblerecibido.Mueble;


        }

        public void abrirmueble(object sender, EventArgs args){
            var d = new NavigationPage(new CreacionDeMueblePage(Mueble,true));
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            App.Navigation.PushAsync(d);
}
        public async void EliminarMueble(object sender, EventArgs args) {
            (this.ParentContext as TiendaViewModel).Eliminando = true;

            bool? respuesta = await MaterialDialog.Instance.ConfirmAsync(message: "Eliminar "+Mueble.Mueble,
                                 title: "Confirmar",
                                 confirmingText: "SI",
                                 dismissiveText: "NO", segundocolor);
            if (respuesta ?? false)
            {
                MessagingCenter.Send<MuebleViewcell, MuebleModel>(this, "Hi", Mueble);
            }
          (this.ParentContext as TiendaViewModel).Eliminando = false;

        }

        public async void expandirmueble(object sender, EventArgs args)
        {
            try
            {

                if ((this.BindingContext as MueblesModel).Expanded == true)
                {

                    (this.BindingContext as MueblesModel).Expanded = false;

                    (this.BindingContext as MueblesModel).Clear();
                }
                else
                {

                    (this.BindingContext as MueblesModel).Expanded = true;

                    (this.BindingContext as MueblesModel).expandir();
                }
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
        }

    }

