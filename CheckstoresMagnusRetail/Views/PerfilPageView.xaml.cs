using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.sqlrepo;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public partial class PerfilPageView : ContentPage
    {
        SQLitemethods sqliterepo = new SQLitemethods();
        ApiRequest repo;
        TableOperationsBase operacion;
        bool isbusy;
        public PerfilPageView()
        {
            InitializeComponent();
            this.Title = "Perfil";
            repo = new ApiRequest();
            operacion = new TableOperationsBase();

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var datos = await SecureStorage.GetAsync("User");
            var usuario = JsonConvert.DeserializeObject<Usuario>(datos);
            NombreUsuario.Text = usuario.Nombre;
            email.Text = usuario.CorreoElectronico;
            telefono.Text = usuario.Telefono;
        }

        public async void reiniciardb(object sender, EventArgs args) {
            sqliterepo.reiniciardb();
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
            //  SecureStorage.Remove("User");
            SecureStorage.RemoveAll();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
           // await Navigation.PopAsync();

        }
    }
}
