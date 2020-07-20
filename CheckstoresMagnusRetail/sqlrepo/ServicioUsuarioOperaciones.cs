using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;

namespace CheckstoresMagnusRetail.sqlrepo
{ 

    public class ServicioUsuarioOperaciones : ITableoperations<ServicioUsuario>
    {



        public async Task<resultfromLocalDB<List<ServicioUsuario>>> ListadeServiciosUsuarios(Usuario parameter)
        {
            resultfromLocalDB<List<ServicioUsuario>> res = new resultfromLocalDB<List<ServicioUsuario>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<ServicioUsuario>().Where(x=>x.UsuarioID==parameter.UsuarioID).ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay servicios para este usuario";
                }
                return res;
            }
            catch (Exception ex)
            {
                res.realizado = false;
                res.Errores = ex.Message;
                Debug.WriteLine(ex.StackTrace);
                return res;
            }
        }
        public override async Task insertarregistro(ServicioUsuario parameter)
        {
            await Reportarproceso("Guardando ServicioUsuario " + parameter.UsuarioID);
           
            await base.insertarregistro(parameter);

        }

        public override async Task clearData()
        {
            try
            {
                await db.ExecuteAsync("Delete from ServicioUsuario");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }



        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de servicios usuario: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
              await  Reportarproceso("Descargando  ServicioUsuarios");
                resultfromAPIUsuarioServicio datos = await repoapi.DescargaUsuariosServicio();
                if (datos.realizado)
                {

                    if (datos.ServicioUsuarios.Count > 0)
                    {
                        await clearData();

                        await insertdata(datos.ServicioUsuarios, this);
                    }
                }
                else
                {
                    await Reportarproceso("Errores en Descarga de ServicioUsuarios"+datos.Errores,
                        true, "", "Descarga de Servicio Usuario");

                    //await mensajetoast(datos.Errores);
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Reportarproceso("Error al descargar Serviciousuarios " + ex.Message + ex.StackTrace,
                    true, "", "Descarga de Serivcio Usuario");

                //  await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<ServicioUsuario>> consultarDatoconcisoAsync(ServicioUsuario parameter)
        {
            throw new NotImplementedException();
        }

        public override Task<resultfromLocalDB<List<ServicioUsuario>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
