using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.ViewModels;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace CheckstoresMagnusRetail.Views.Vewscontents
{
    public partial class LoginView : ContentView
    {
        SQLitemethods sqliterepo = new SQLitemethods();
        ApiRequest api = new ApiRequest();

        public LoginView()
        {
            InitializeComponent();
            maskedEdit.ValueMaskFormat = Syncfusion.XForms.MaskedEdit.MaskFormat.ExcludePromptAndLiterals;

        }

        public async void login_clickAsync(object sender, EventArgs args)
        {
            MaterialLoadingDialogConfiguration mo = new MaterialLoadingDialogConfiguration();
            mo.BackgroundColor = (Color)Application.Current.Resources["azul"];
            mo.MessageTextColor = (Color)Application.Current.Resources["blanco"];
            mo.TintColor = (Color)Application.Current.Resources["mist"];

            if (!string.IsNullOrEmpty(usuario.Text) && !string.IsNullOrEmpty((string)maskedEdit.Value))
            {
                using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Autentificando", mo))
                {
                    await loginUsuario(usuario.Text, (string)maskedEdit.Value);
                }
            }
            else
            {
               (this.BindingContext as LoginModel).Errorusuario = true;
                (this.BindingContext as LoginModel).Errorpassword = true;
            }

        }



     
        public async Task loginUsuario(string Usuario, string pass)
        {

            sqliterepo.createdb();
            var color = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration();
            color.BackgroundColor = (Color)Application.Current.Resources["azul"];
            var resultado = await api.LoginUsuario(Usuario, pass);
            if (resultado.realizado)
            {
                //await App.Navigation.PushAsync(new TabbedPages());

                if (resultado.Result.Autenticado)
                {
                    resultado.Result.Password = pass;
                    await guardarensecurityAsync(resultado.Result);
                    await Navigation.PushAsync(new TabbedPages());
                }
                else
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: resultado.Errores,
                            actionButtonText: "OK",
                            msDuration: 20000, color);
                }
            }
            else
            {

                /*
                await MaterialDialog.Instance.SnackbarAsync(message: "No se pudo establecer la conexion",
                                            actionButtonText: "OK",
                                           
                                            msDuration: 3000, color);*/

                UsuarioOperacion up = new UsuarioOperacion();
                var second = await up.LoginLocal(new Usuario { NombreUsuario = Usuario, Password = pass });
                if (second.realizado)
                {
                    second.Result.Password = pass;

                    await guardarensecurityAsync(second.Result);
                    await Navigation.PushAsync(new TabbedPages());

                }
                else
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: second.Errores,
                             actionButtonText: "OK",
                             msDuration: 20000, color);
                }

            }
            // new NavigationPage(new Views.LoginPage())
        }

        public async Task guardarensecurityAsync(Usuario usuario)
        {
            usuario.Foto = null;
            var data = JsonConvert.SerializeObject(usuario);
            await SecureStorage.SetAsync("User", data);
        }
    }
}
