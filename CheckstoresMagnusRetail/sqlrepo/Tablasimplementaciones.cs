using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.sqlrepo
{

    public class UsuarioOperacion : ITableoperations<Usuario>
    {
        public override async Task<resultfromLocalDB<Usuario>> consultarDatoconcisoAsync(Usuario parameter)
        {
           // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            resultado.Result = await db.Table<Usuario>().FirstOrDefaultAsync(x => x.NombreUsuario == parameter.NombreUsuario && x.Password == parameter.Password);
            return resultado;
        }
        public async Task<resultfromLocalDB<Usuario>> LoginLocal(Usuario parameter) {
            //  resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                var m = db.Table<Usuario>().Where(x => x.NombreUsuario == parameter.NombreUsuario);

                if (m.CountAsync().Result > 0)
                {
                    resultado.realizado = true;
                    resultado.Result = await m.FirstOrDefaultAsync(x => x.Password == parameter.Password);
                    if (resultado.Result == null)
                    {
                        resultado.realizado = false;
                        resultado.Errores = "contrasena incorrecta";
                    }
                }
                else
                {
                    resultado.realizado = false;
                    resultado.Errores = "No se encontro el Usuario";
                }
            }
            catch  {
                resultado.realizado = false;
                resultado.Errores = "No existen datos locales";
            }
            return resultado;
        }

        public override async Task clearData() {
            try
            {
                await db.ExecuteAsync("Delete from Usuario");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
    
        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de usuarios: ");
            if (prueba)
                return;
            try
            {
               await Reportarproceso("Descargando Usuarios");
                   resultfromAPIUsuario datos = await repoapi.DescargaCatalogosUsuarios();
                if (datos.realizado)
                {
                    if (datos.Usuarios.Count>0) {
                        await clearData();

                        await insertdata(datos.Usuarios, new UsuarioOperacion());
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Usuarios " + datos.Errores, true, "", "Descarga de usuarios");
                }
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Usuarios " + ex.Message + ex.StackTrace, true, "", "Descarga de Usuarios");

                //   await mensajetoast(ex.Message);
            }

        }

        public override Task<resultfromLocalDB<List<Usuario>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
