using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using CheckstoresMagnusRetail.Views.Vewscontents;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace CheckstoresMagnusRetail.Views
{
    public partial class LoginPage : ContentPage
    {
       // TabbedPages view;
        ApiRequest api = new ApiRequest();
        SQLitemethods sqliterepo = new SQLitemethods();
        LoginModel context;
        bool appeareaces;
        bool iniciosesion;
        TableOperationsBase ope;
        public LoginPage()
        {
            InitializeComponent();
            appeareaces = false;
            iniciosesion = false;

            this.BindingContext = context = new LoginModel();
            TableOperationsBase ope = new TableOperationsBase();
        }

        public async Task loginUsuario()
        {
            var datos =await SecureStorage.GetAsync("User");
            if (datos != null)
            {
                //var usuari = JsonConvert.DeserializeObject<Usuario>(datos);
                /*
                string Usuario = usuari.Nombre;
                string pass = usuari.Password;
                sqliterepo.createdb();
                var color = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration();
                color.BackgroundColor = (Color)Application.Current.Resources["azul"];
                var resultado = await api.LoginUsuario(Usuario, pass);
                if (resultado.realizado)
                {
                    if (resultado.Result.Autenticado)
                    {
                        resultado.Result.Password = pass;
                        */
                        iniciosesion = true;
                        
                        //await guardarensecurityAsync(resultado.Result);
                        await Navigation.PushAsync(new TabbedPages());
                /*
                    }
                    else
                    {
                        iniciosesion = false;
                       await ope.Reportarproceso("Error Login a api "+resultado.Errores,true,Usuario+"::"+pass,"Login");
                    }

                }
                else
                {
                    UsuarioOperacion up = new UsuarioOperacion();
                    var second = await up.LoginLocal(new Usuario { NombreUsuario = Usuario, Password = pass });
                    if (second.realizado)
                    {
                        second.Result.Password = pass;
                        iniciosesion = true;

                        await guardarensecurityAsync(second.Result);
                        await App.Navigation.PushAsync(new TabbedPages());

                    }
                    else
                    {
                        await ope.Reportarproceso("Error Login local " + resultado.Errores, true, Usuario + "::" + pass, "Login");

                        iniciosesion = false;
                    }

                }*/
            }
            else {
                iniciosesion = false;
            }
           }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
          await  loginUsuario();


            if (appeareaces == false && iniciosesion==false ) {
                this.Content = new LoginView();
                appeareaces = true ;
            }

        }


        public async Task guardarensecurityAsync(Usuario usuario) {
            usuario.Foto = null;
          var data=  JsonConvert.SerializeObject(usuario);
            await SecureStorage.SetAsync("User", data);
        }
    }
}
