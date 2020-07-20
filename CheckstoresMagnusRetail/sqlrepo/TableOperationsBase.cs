using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.sqlrepo
{
    public class TableOperationsBase
    {
        public string path;
        public string DispositivoID;
        public string DispositivoName;
        public int UsuarioID;
        public string usuarioname;
        public TableOperationsBase() {
            DispositivoName = DeviceInfo.Name;
            IDevice device = DependencyService.Get<IDevice>();
            DispositivoID = device.GetIdentifier();
            getuser();
        }
        public async Task getuser()
        {
            var datos = await SecureStorage.GetAsync("User");
            var usuari = JsonConvert.DeserializeObject<Usuario>(datos);
            UsuarioID = usuari.UsuarioID;
            usuarioname = usuari.NombreUsuario;
        }
        public async Task Reportarproceso(string texto,bool error=false,object parametros=null,string flujo="",string stacktrace="",
            [CallerMemberName] string evento="")
        {
            try
            {
                if(error==true)
                    await escribirerror(texto,evento, flujo, parametros,stacktrace);
                MessagingCenter.Send(this, "Hi", texto);
            }
            catch { }
        }



        public async Task mensajetoast(string Error)
        {
            try
            {
               // var color = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration();
                // color.BackgroundColor = (Color)Application.Current.Resources["azul"];
                await MaterialDialog.Instance.SnackbarAsync(message: Error,
                
                msDuration: 3000);
            }
            catch { }
        }
        public async Task escribirerror(string error,string evento,string flujo,object param,string stacktrace) {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                  DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt");
            using (StreamWriter sw = (File.Exists(fileName)) ? File.AppendText(fileName) : File.CreateText(fileName))
            {
                var parametros = JsonConvert.SerializeObject(param);
               
                var errorstring = JsonConvert.SerializeObject(new ErrorLog
                {
                    Usuario = UsuarioID.ToString()+"::"+usuarioname+"::"+DispositivoName+"::"+DispositivoID,
                    FechaAlta= DateTime.Now,
                    Descripcion="Error App Checkstore",
                    Proyecto="Checkstore MOBILEAPP",
                   TipoID=2,
                   Evento=evento,
                   Datos=error,
                   Parametros=parametros,
                   Flujo=flujo,
                   Source=stacktrace
                }) ;

               // var errorstring= 
                await sw.WriteLineAsync(errorstring);
            }
        }


    }
}
