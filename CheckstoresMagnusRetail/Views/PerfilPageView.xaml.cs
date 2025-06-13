using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.sqlrepo;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.Views
{
    public partial class PerfilPageView : ContentPage
    {
        SQLitemethods sqliterepo = new SQLitemethods();
        ApiRequest repo;
        FotoProductoOperaciones fotorepo = new FotoProductoOperaciones();
        TableOperationsBase operacion;
        bool isbusy;
        bool clicked;
        public PerfilPageView()
        {
            InitializeComponent();
            this.Title = "Perfil";
            repo = new ApiRequest();
            operacion = new TableOperationsBase();
            clicked = false;

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var datos = await SecureStorage.GetAsync("User");
            var usuario = JsonConvert.DeserializeObject<Usuario>(datos);
            if (usuario != null)
            {
                NombreUsuario.Text = usuario.Nombre;
                email.Text = usuario.CorreoElectronico;
                telefono.Text = usuario.Telefono;
            }
        }

        public async void reiniciardb(object sender, EventArgs args) {
            sqliterepo.reiniciardb();
        }

        string GetPath()
        {
            string dbName = "Checkstorev2.db3";
            string path = "";
            var ruta = Task.Run(async () => {
                path = await SecureStorage.GetAsync("rutadb");
            }
              );
            Task.WaitAll(ruta);
            if (path == "" || path == null)
                path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return path;
        }

        public async void cargarerrores(object sender, EventArgs args) {
            if (isbusy)
                return;
            isbusy = true;
            string line = "";
            await desactivarboton();
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt");
           // int counter = 0;
            if (!File.Exists(fileName))
            {
                isbusy = false;
                await activarboton();
                return;
            }
            int counter = 0;
            int batch = 0;
            System.IO.StreamReader file =
                new System.IO.StreamReader(fileName);
            List<Task> tareas = new List<Task>();
            while ((line = file.ReadLine()) != null)
            {
              tareas.Add(repo.postapiAeeorAsync(line));
                // if (!d.realizado)
                //   await operacion.mensajetoast(d.Errores);
                counter++;
                if (counter == 100) {
                    batch++;
                    botoncargaerrores.Text = "Cargando batch de errores "+batch;
                 await  Task.WhenAll(tareas);
                    tareas.Clear();
                    counter = 0;
                }
               
                
            }
            if (tareas.Count > 0)
                await Task.WhenAll(tareas);
            File.Delete(fileName);
           await activarboton();
            isbusy = false;
        }


        public async Task desactivarboton() {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                botoncargaerrores.BackgroundColor = (Color)Application.Current.Resources["azul"];
                botoncargaerrores.TextColor = Color.White;
                botoncargaerrores.Text = "Cargando Errores...";
            });
        }
        public async Task activarboton (){
            await Device.InvokeOnMainThreadAsync(() =>
            {
                botoncargaerrores.BackgroundColor = (Color)Application.Current.Resources["Gris"];
                botoncargaerrores.TextColor = (Color)Application.Current.Resources["azul"];

                botoncargaerrores.Text = "Cargar Log de Errores";
            });
        }

        public async void logout(object sender, EventArgs args) {
              SecureStorage.Remove("User");
            //SecureStorage.RemoveAll();
           // 
                var d= new NavigationPage(new LoginPage());
            d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
            Application.Current.MainPage = d;
            App.Navigation = Application.Current.MainPage.Navigation;
           // await Navigation.PopAsync();

        }

        public async  void CopyDatabase(object sender, EventArgs args) {
            if (clicked)
                return;
            clicked = true;
            var bytes = System.IO.File.ReadAllBytes(GetPath());
            var fileCopyName = string.Format("Databasecheckstore_{0:dd-MM-yyyy_HH-mm-ss-tt}.db", System.DateTime.Now);
            try
            {
                var filepathr = "";
                System.IO.File.WriteAllBytes(filepathr, bytes);
                await MaterialDialog.Instance.SnackbarAsync(message: "Guardado: "+ filepathr,
actionButtonText: "OK",
msDuration: 000);

            }
            catch(Exception ex) {
                await MaterialDialog.Instance.SnackbarAsync(message: ex.Message,
actionButtonText: "OK",
msDuration: 3000);
            }

            clicked = false;
        }


        public async void CopyLog(object sender, EventArgs args)
        {
            var bytes = System.IO.File.ReadAllBytes(GetPath());
            var fileCopyName = string.Format("logcheckstore{0:dd-MM-yyyy_HH-mm-ss-tt}.txt", System.DateTime.Now);
            try
            {
                //var filepathr = Path.Combine((string)Android.OS.Environment.GetExternalStoragePublicDirectory(""), fileCopyName);
                var filepathr = "";
                System.IO.File.WriteAllBytes(filepathr, bytes);
                await MaterialDialog.Instance.SnackbarAsync(message: "Guardado: " + filepathr,
actionButtonText: "OK",
msDuration: 000);

            }
            catch (Exception ex)
            {
                await MaterialDialog.Instance.SnackbarAsync(message: ex.Message,
actionButtonText: "OK",
msDuration: 3000);
            }
        }

        public async void reiniciarfotos(object sender, EventArgs args) {
            if (clicked)
                return;
            clicked = true;
            reiniciarfotosbutton.BackgroundColor = (Color)Application.Current.Resources["azul"];
            reiniciarfotosbutton.TextColor = Color.White;
            reiniciarfotosbutton.Text = "recargando fotos...";
            await fotorepo.CargarDatosdelayoutalternative();

            reiniciarfotosbutton.BackgroundColor = (Color)Application.Current.Resources["Gris"];
            reiniciarfotosbutton.TextColor = (Color)Application.Current.Resources["azul"];
            reiniciarfotosbutton.Text = "recargar fotos";
            await MaterialDialog.Instance.SnackbarAsync(message: "Recarga de fotos completada",
actionButtonText: "OK",
msDuration: 3000);
            clicked = false;
        }
    }
}
