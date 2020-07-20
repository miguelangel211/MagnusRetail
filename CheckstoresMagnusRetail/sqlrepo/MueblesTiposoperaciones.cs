using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;

namespace CheckstoresMagnusRetail.sqlrepo
{
  

    public class MueblesTiposoperaciones : ITableoperations<MuebleTipo>
    {



        public override async Task<resultfromLocalDB<MuebleTipo>> consultarDatoconcisoAsync(MuebleTipo parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<MuebleTipo>().FirstOrDefaultAsync();
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

        public async Task<resultfromLocalDB<List<MuebleTipo>>> ListadeTiposMuebles()
        {
            resultfromLocalDB<List<MuebleTipo>> res = new resultfromLocalDB<List<MuebleTipo>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<MuebleTipo>().ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No se encontraron muebles";
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


        public override async Task clearData()
        {
            await db.ExecuteAsync("Delete from MuebleTipo");
        }
        public override async Task insertarregistro(MuebleTipo parameter)
        {
            await Reportarproceso("Guardando Mueble Tipo " + parameter.MuebleTipoID);
            await base.insertarregistro(parameter);

        }


        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de Muebles tipo: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Tipos Muebles" );

                resultfromAPIMuebleTipo datos = await repoapi.DescargaTiposMuebles();
                if (datos.realizado)
                {
                    if (datos.MuebleTipo.Count > 0)
                    {
                        await clearData();

                        await insertdata(datos.MuebleTipo, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar  Tipos Mueble " + datos.Errores,
                        true, "", "Descarga de tipos de mueble");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Reportarproceso("Error al descargar Tipos Mueble " + ex.Message + ex.StackTrace,
                    true, "", "Descarga de tipos de mueble");

                // await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<List<MuebleTipo>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
