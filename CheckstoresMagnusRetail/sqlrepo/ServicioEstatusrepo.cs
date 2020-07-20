using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;

namespace CheckstoresMagnusRetail.sqlrepo
{
   

    public class ServicioEstatusrepo : ITableoperations<ServicioEstatus>
    {



        public override async Task<resultfromLocalDB<ServicioEstatus>> consultarDatoconcisoAsync(ServicioEstatus parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<ServicioEstatus>().FirstOrDefaultAsync();
                if (resultado.Result != null)
                {
                    resultado.realizado = true;
                }
                else
                {
                    resultado.realizado = false;
                }
                return resultado;
            }
            catch
            {
                resultado.realizado = false;
                return resultado;
            }
        }

        public async Task<resultfromLocalDB<List<ServicioEstatus>>> ListadeServiciosEstatus()
        {
            resultfromLocalDB<List<ServicioEstatus>> res = new resultfromLocalDB<List<ServicioEstatus>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<ServicioEstatus>().ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No Estatus";
                }
                return res;
            }
            catch(Exception ex)
            {
                res.realizado = false;
                res.Errores = ex.Message;
                Debug.WriteLine(ex.StackTrace);
                return res;
            }
        }


        public override async Task clearData()
        {
            try
            {
                await db.ExecuteAsync("Delete from ServicioEstatus");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        public override async Task insertarregistro(ServicioEstatus parameter)
        {
            await Reportarproceso("Guardando Estatus " + parameter.ServicioEstatusID);
            
            await base.insertarregistro(parameter);

        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de estatus: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Estatus ");

                resultfromAPIStatusServicio datos = await repoapi.Descargaestatusservicio();
                if (datos.realizado)
                {

                    if (datos.ServicioEstatus.Count > 0)
                    {
                        await clearData();

                        await insertdata(datos.ServicioEstatus, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Estatus" + datos.Errores,
                        true, "", "Descarga de estatus");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Estatus" + ex.Message + ex.StackTrace,
                    true, "", "Descarga de Estatus");

                Debug.WriteLine(ex.Message);

              //  await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<List<ServicioEstatus>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
