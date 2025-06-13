using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.ViewModels
{
    public abstract class BaseViewModel : TableOperationsBase,INotifyPropertyChanged
    {

        ApiRequest repoapi = new ApiRequest();
        public List<string> logtext;
        public List<string> LOGtEXT { get { return logtext; } set { logtext = value;OnPropertyChanged(); } }
        XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration color = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration();
        XF.Material.Forms.UI.Dialogs.Configurations.MaterialAlertDialogConfiguration segundocolor = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialAlertDialogConfiguration();

       /* public async Task mensajetoast(string Error) {
            await MaterialDialog.Instance.SnackbarAsync(message: Error,
            actionButtonText: "OK",
            msDuration: 3000, color);
        }
        */
        bool isBusy = false;
        string operacionactiva;
       public string OperacionActiva { get { return operacionactiva; } set { operacionactiva = value; OnPropertyChanged(); } }

        public bool IsBusy{get { return isBusy; }set { SetProperty(ref isBusy, value); }}

        public string imagensincro;
        public string ImagenSincro { get { return imagensincro; } set { imagensincro = value;OnPropertyChanged(); } }

        public bool sincronizaciondata;

        public bool SincronizacionData {
            get { return sincronizaciondata; }
            set { sincronizaciondata = value; Botoncerraractivo=!value; if (value) { ImagenSincro = "synccloud.png"; } else { ImagenSincro = "cloudstorage.png"; }  OnPropertyChanged(); }
        }

        public double opacitylist;
        public double Opacitylist { get { return opacitylist; } set { opacitylist = value;OnPropertyChanged(); } }
        public bool listahabilitada { get; set; }

        public bool ListaHabilitada {
            get { return listahabilitada; }
            set { listahabilitada = value;if (value) { Opacitylist = 1; } else { Opacitylist = .4; }  OnPropertyChanged(); }
        }
        public bool logginvisible;
        public bool Logginvisible { get { return logginvisible; } set { logginvisible = value;OnPropertyChanged(); } }

        public bool botoncerrar;
        public bool Botoncerraractivo { get { return botoncerrar; } set { botoncerrar = value;OnPropertyChanged(); } }
        public Command cargardata;
       
        public Command CargarData { get { return cargardata; } set { cargardata = value; OnPropertyChanged(); } }

        private bool issincronizing;
        public bool Issincronizando { get { return issincronizing; } set { issincronizing = value; OnPropertyChanged(); } }

        public bool SetProperty<T>(ref T backingStore, T value,[CallerMemberName]string propertyName="",Action onChanged=null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;

        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BaseViewModel()
        {
            color.BackgroundColor = (Color)Application.Current.Resources["azul"];

            segundocolor.BackgroundColor = (Color)Application.Current.Resources["azul"];
            segundocolor.MessageTextColor = Color.White;
            segundocolor.TitleTextColor = (Color)Application.Current.Resources["mist"];
            segundocolor.TintColor= (Color)Application.Current.Resources["mist"];
            CargarData = new Command(async()=>await CargarDatos());
            ListaHabilitada = true;
            LOGtEXT = new List<string>();
            Opacitylist = 1;
            MessagingCenter.Subscribe<TableOperationsBase, string>(this, "Hi", async (sender, arg) =>
            {
                try
                {
                    OperacionActiva = arg;
                    LOGtEXT.Add(DateTime.Now + ": " + arg);
                }
                catch { }
            });
        }

        public void logaddtext(string arg) {
            try
            {
                OperacionActiva = arg;
                LOGtEXT.Add(DateTime.Now + ": " + arg);
            }
            catch { }
        }
        public virtual Task CargarDatos() {
            return Task.Run(()=> { });
        }

        public void sincronizardata() {
            Task.Run(()=> { });
        }

 
        public async Task recargarDatos()
        {
            if (!Issincronizando)
                return;
            var api =await SecureStorage.GetAsync("rutaapi");
            OperacionActiva = "Iniciando Sincronizacion a "+api;

            SincronizacionData = true;
            Issincronizando = false;
            IsBusy = true;
            LOGtEXT.Clear();
            ListaHabilitada = false;
            Logginvisible = true;
            await Task.Delay(20);

            try
            {
              //  var fotosprod = new FotoProductoOperaciones();

                
                var userop = new UsuarioOperacion();
                var prodop = new ProductoOperacion();
                var servicios = new ServiciosOperaciones();
                var estatus = new ServicioEstatusrepo();
                var tiposmuebles = new MueblesTiposoperaciones();
                var muebles = new ServicioMueblesOperaciones();
                var fotos = new muebleFotgrafiasOperaciones();
                var lyout = new LayoutFotosOperaciones();
                var fotosprod = new FotoProductoOperaciones();
                var categorias = new CategoriasOperaciones();
                var tramos = new TramosOperaciones();
                var niveles = new TramoNivelesOperaciones();
                var nivelesmuebleservicioproducto = new ServicioProductoNivelOperaciones();
                var catogoriastramosnivel = new MuebleTramoNivelCategoriaOperaciones();
                var serviciousuario = new ServicioUsuarioOperaciones();

                await muebles.CargarDatosdemueble();
           
                await tramos.CargarDatos();
                await niveles.CargarDatos();
                await catogoriastramosnivel.CargarDatos();
                await prodop.CargarDatos();
                await fotos.CargarDatosdelayout();
                await lyout.CargarDatosdelayout();
                await nivelesmuebleservicioproducto.CargarDatos();
                await fotosprod.CargarDatosdelayout();
                await servicios.CargaConcluirServicio();
                await categorias.SincronizaciondesdeAPI();
                await servicios.SincronizaciondesdeAPI();
                await muebles.SincronizaciondesdeAPI();
                await nivelesmuebleservicioproducto.SincronizaciondesdeAPI();
                await userop.SincronizaciondesdeAPI();
                await prodop.SincronizaciondesdeAPI();
                await estatus.SincronizaciondesdeAPI();
                await tiposmuebles.SincronizaciondesdeAPI();
                await serviciousuario.SincronizaciondesdeAPI();
                await tramos.SincronizaciondesdeAPI();
                await lyout.SincronizaciondesdeAPI();
                await niveles.SincronizaciondesdeAPI();
                await catogoriastramosnivel.SincronizaciondesdeAPI();
                await fotos.SincronizaciondesdeAPI();
               
                bool? respuesta=   await MaterialDialog.Instance.ConfirmAsync(message: "Descargar Fotos de Producto",
                                     title: "Confirmar Descarga",
                                     confirmingText: "SI",
                                     dismissiveText: "NO",segundocolor);
                if (respuesta ?? false) {
                    await fotosprod.SincronizaciondesdeAPI();
                }
            }
            catch (Exception ex) {
                logaddtext(ex.Message+" : "+ex.StackTrace);
            }
                OperacionActiva = "Finalizado";

            ListaHabilitada = true;

            SincronizacionData = false;
            Issincronizando = true;
            IsBusy = false;
            await CargarDatos();

        }




    }
}
