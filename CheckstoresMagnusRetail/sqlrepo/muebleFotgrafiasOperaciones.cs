using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{
    
    public class muebleFotgrafiasOperaciones : ITableoperations<ServicioMuebleImagen>
    {



        public override async Task<resultfromLocalDB<ServicioMuebleImagen>> consultarDatoconcisoAsync(ServicioMuebleImagen parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<ServicioMuebleImagen>().FirstOrDefaultAsync();
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

        public override async Task insertarregistro(ServicioMuebleImagen parameter)
        {
            try
            {
                await Reportarproceso("Guardando Foto: " + parameter.ServicioMuebleImagenID + " de mueble: " + parameter.ServicioMuebleID);
            }
            catch { }
            try
            {
                parameter.FechaHoraLocal = DateTime.Now;
                parameter.UsuarioLocalID = UsuarioID;
                parameter.DispositivoNombre = DispositivoName;
                parameter.DispositivoID = DispositivoID;
                await base.insertarregistro(parameter);
            }
            catch { }
        }
        public async Task CargarDatosdelayout()
        {
            var prueba = await probarred("Carga de fotos de muebles: ");
            if (prueba)
                return;
            var m = await db.Table<ServicioMuebleImagen>().Where(x => (x.Sincronizado == false || x.Sincronizado == null)&&(x.ServicioMuebleID!=0&& x.ServicioMuebleID!=null)).ToListAsync();
            if (m.Count > 0)
            {

                foreach (var i in m)
                {
                   // i.DispositivoID = DispositivoID;
                   // i.DispositivoNombre = DispositivoName;
                    await Reportarproceso("Carga de Foto de mueble local " + i.ServicioMuebleImagenLocalID);
                    var r = await repoapi.CargaMuebleImagenPOST(i);
                    if (r.realizado)
                    {
                        try
                        {
                            var tempm = await db.Table<ServicioMuebleImagen>().FirstOrDefaultAsync(x => x.ServicioMuebleImagenLocalID == i.ServicioMuebleImagenLocalID);
                            tempm.Sincronizado = true;
                            await db.UpdateAsync(tempm);
                        }
                        catch(Exception ex) {
                            await Reportarproceso("Error en actualizacion de Foto de mueble sinconizado " + ex.Message+ex.StackTrace,
                                 true,i.ServicioMuebleImagenLocalID, "Carga de foto de mueble");

                        }

                    }
                    else {
                        await Reportarproceso("Error Carga de Foto de mueble " + resultado.Errores,
                             true, i.ServicioMuebleImagenLocalID, "Carga de foto de mueble");

                    }
                }
            }
            else
            {

            }
        }

        public async Task<resultfromLocalDB<List<ServicioMuebleImagen>>> ListadeTiposMuebles()
        {
            resultfromLocalDB<List<ServicioMuebleImagen>> res = new resultfromLocalDB<List<ServicioMuebleImagen>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<ServicioMuebleImagen>().ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No se encontraron fotos";
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
            try
            {
                await db.ExecuteAsync("Delete from ServicioMuebleImagen where Sincronizado=1");
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }


        public async Task GUardarimagenesdemueblesinasignar(ServicioMueble parameter) {

            var mueble = await db.Table<ServicioMuebleImagen>().Where(x => x.ServicioMuebleLocalID == 0 && x.ServicioMuebleID == 0)
.ToListAsync();
        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de fotos de mueble: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Fotos de Muebles ");

                resultfromAPIFotoMueble datos = await repoapi.DescargaMueblesServicioImagen();
                if (datos.realizado)
                {
                    if (datos.MUebleImagen.Count > 0)
                    {
                       await clearData();

                        await insertdata(datos.MUebleImagen, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Fotos de Muebles " + datos.Errores
                        , true, "", "Descarga de fotos de mueble");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Fotos de Muebles " + ex.Message+ex.StackTrace
                    , true,"" , "Descarga de fotos de muebles");


              //  await mensajetoast(ex.StackTrace);
            }

        }

        public override async Task<resultfromLocalDB<List<ServicioMuebleImagen>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<ServicioMuebleImagen>> resultado = new resultfromLocalDB<List<ServicioMuebleImagen>>() { realizado = true, Errores = "" };
            try
            {
                var parametro = (parameter as ServicioMueble);
                //  resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
                var m =  await db.Table<ServicioMuebleImagen>()
                   .Where(

                    x =>(x.ServicioMuebleID==parametro.ServicioMuebleID && parametro.ServicioMuebleID != 0 && parametro.ServicioMuebleID!=null ) ||
                    (parametro.ServicioMuebleLocalID==x.ServicioMuebleLocalID && parametro.ServicioMuebleLocalID!=null)||(
                    x.ServicioMuebleID==0 && x.ServicioMuebleLocalID==0 && parametro.ServicioMuebleLocalID==null
                    )
                    )
                   
                    .ToListAsync();
               // m = m.Where(x => x.ServicioMuebleID == (parameter as ServicioMueble).ServicioMuebleID).ToList();
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

        public async Task borrarfoto(int ID) {
            var imagenaborrar = await db.Table<ServicioMuebleImagen>().FirstOrDefaultAsync(x=>x.ServicioMuebleImagenLocalID==ID);
           await db.DeleteAsync(imagenaborrar);
        }
    }
}
