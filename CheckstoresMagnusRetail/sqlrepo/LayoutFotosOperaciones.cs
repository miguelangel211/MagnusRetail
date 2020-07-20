using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{
    
    public class LayoutFotosOperaciones : ITableoperations<ServicioLayout>
    {



        public override async Task<resultfromLocalDB<ServicioLayout>> consultarDatoconcisoAsync(ServicioLayout parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<ServicioLayout>().FirstOrDefaultAsync();
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


        public async Task CargarDatosdelayout()
        {
            var prueba = await probarred("Carga de layouts: ");
            if (prueba)
                return;
            var m = await db.Table<ServicioLayout>().Where(x => x.Sincronizado == false || x.Sincronizado == null).ToListAsync();
            if (m.Count > 0)
            {

                foreach (var i in m)
                {
                   // i.DispositivoID = DispositivoID;
                   // i.DispositivoNombre = DispositivoName;
                    await Reportarproceso("Carga de foto de layout local " + i.ServicioLayoutLocalID);

                    var r = await repoapi.CargaLayoutPOST(i);
                    if (r.realizado)
                    {
                        try
                        {
                            await db.ExecuteAsync("update ServicioLayout set Sincronizado=1 where ServicioLayoutLocalID=?",i.ServicioLayoutLocalID);
                            /*
                            var foto = dbsincrona.Table<ServicioLayout>().FirstOrDefault(x => x.ServicioLayoutLocalID == i.ServicioLayoutLocalID);
                            foto.Sincronizado = true;
                          await  db.UpdateAsync(foto);
                          */

                        }
                        catch (Exception ex) {
                            await Reportarproceso("Error de carga de Layout " + ex.Message + ex.StackTrace
                                , true, i.ServicioLayoutLocalID, "Carga de Layout");

                        }
                    }
                    else {
                        await Reportarproceso("Error de carga de Layout " + resultado.Errores,
                            true, i.ServicioLayoutLocalID, "Carga de Layout");

                    }
                }
            }
            else
            {

            }
        }

        public async Task<resultfromLocalDB<List<ServicioLayout>>> ListadeTiposMuebles()
        {
            resultfromLocalDB<List<ServicioLayout>> res = new resultfromLocalDB<List<ServicioLayout>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<ServicioLayout>().ToListAsync();

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
            await db.ExecuteAsync("Delete from ServicioLayout where Sincronizado=1");
        }

        public override async Task insertarregistro(ServicioLayout parameter)
        {
            await Reportarproceso("Guardando Layout " + parameter.ServicioLayoutID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoID = DispositivoID;
            parameter.DispositivoNombre = DispositivoName;
            await base.insertarregistro(parameter);

        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de fotos layout: x");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargar Fotos de Layout" );

                resultfromAPIFotoLayout datos = await repoapi.DescargaLayoutImagen();
                if (datos.realizado)
                {
                    await clearData();

                    if (datos.LayoutImagen.Count > 0)
                    {
                        await insertdata(datos.LayoutImagen, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Fotos de Layout " + datos.Errores,
                        true, "", "Descarga fotos de layout");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Fotos de Layout " + ex.Message + ex.StackTrace,
                    true, "", "Decarga fotos de layout"
                    );

                Debug.WriteLine(ex.Message);

                //await mensajetoast(ex.StackTrace);
            }

        }

        public override async Task<resultfromLocalDB<List<ServicioLayout>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<ServicioLayout>> resultado = new resultfromLocalDB<List<ServicioLayout>>() { realizado = true, Errores = "" };
            try
            {
                //  resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
                var m =  dbsincrona.Table<ServicioLayout>().ToList().Where(x=>x.ServicioID== (parameter as Servicio).ServicioID).ToList();

                if (m.Count > 0)
                {
                    resultado.Result = m;
                    resultado.realizado = true;

                }
                else
                {
                    resultado.realizado = false;
                    resultado.Errores = "No se encontraron Fotos";
                }
                return resultado;
            }
            catch
            {
                resultado.realizado = false;
                resultado.Errores = "No se encontraron Fotos";
                return resultado;
            }
        }


        public async Task borrarfoto(int ID)
        {
            var imagenaborrar = await db.Table<ServicioLayout>().FirstOrDefaultAsync(x => x.ServicioLayoutLocalID == ID);
            await db.DeleteAsync(imagenaborrar);
        }
    }
}
