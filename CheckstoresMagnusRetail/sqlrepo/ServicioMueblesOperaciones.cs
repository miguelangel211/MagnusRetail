using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.DataModels;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{

    public class ServicioMueblesOperaciones : ITableoperations<ServicioMueble>
    {



        public override async Task<resultfromLocalDB<ServicioMueble>> consultarDatoconcisoAsync(ServicioMueble parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<ServicioMueble>().FirstOrDefaultAsync(
                    x => (x.ServicioMuebleID == parameter.ServicioMuebleID && parameter.ServicioMuebleID != 0 && parameter.ServicioMuebleID != null) ||
                    (x.ServicioMuebleLocalID == parameter.ServicioMuebleLocalID && parameter.ServicioMuebleLocalID != 0 && parameter.ServicioMuebleLocalID != null)
                    );
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

        public async Task<resultfromLocalDB<List<ServicioMueble>>> ListadeMuebles(Servicio parametro)
        {
            resultfromLocalDB<List<ServicioMueble>> res = new resultfromLocalDB<List<ServicioMueble>>() { realizado = true, Errores = "" };

            try
            {

                //  var m = await db.Table<ServicioMueble>().ToListAsync();
                var m = await db.Table<ServicioMueble>().Where(x=>x.ServicioID== parametro.ServicioID).OrderBy(x=>x.ServicioMuebleLocalID).ToListAsync();
                
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
            try
            {
                await db.ExecuteAsync("Delete from ServicioMueble where Sincronizado=1");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
        public override async Task insertarregistro(ServicioMueble parameter)
        {
            await Reportarproceso("Guardando Mueble " + parameter.ServicioMuebleID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            parameter.DispositivoID = DispositivoID;

            await base.insertarregistro(parameter);

        }

        public async Task EliminarMueble(MuebleModel mueble) {
           await db.ExecuteAsync("Delete from ServicioMueble where ServicioMuebleLocalID =" + mueble.ServicioMuebleLocalID);
            await db.ExecuteAsync("Delete from ServicioMuebleTramo where ServicioMuebleLocalID =" + mueble.ServicioMuebleLocalID);
            await db.ExecuteAsync("Delete from ServicioMuebleImagen where ServicioMuebleLocalID =" + mueble.ServicioMuebleLocalID);
            var niveles =await db.Table<ServicioMuebleTramoNivel>().Where(x=>x.ServicioMuebleLocalID == mueble.ServicioMuebleLocalID).ToListAsync();
            foreach (var niv in niveles)
            {
                await db.ExecuteAsync("Delete from ServicioMuebleTramoNivelCategoria where ServicioMuebleTramoNivelLocalID =" + niv.ServicioMuebleTramoNivelLocalID);
                await db.ExecuteAsync("Delete from ServicioMuebleProductoNivel where ServicioMuebleTramoNivelLocalID =" + niv.ServicioMuebleTramoNivelLocalID);

               // var tramos = dbsincrona.Table<ServicioMuebleTramoNivelCategoria>().
               //     Where(x => x.ServicioMuebleTramoNivelLocalID == niv.ServicioMuebleTramoNivelLocalID);
               // var productonivel = dbsincrona.Table<ServicioMuebleProductoNivel>().
               //     Where(x => x.ServicioMuebleTramoNivelLocalID == niv.ServicioMuebleTramoNivelLocalID);

                await db.DeleteAsync(niv);
            }
            /*
            var nivelesserver = await db.Table<ServicioMuebleTramoNivel>().Where(x => x.ServicioMuebleID == mueble.MuebleID).ToListAsync();
            foreach (var niv in niveles)
            {
                await db.ExecuteAsync("Delete from ServicioMuebleTramoNivelCategoria where ServicioMuebleTramoNivelID =" + niv.ServicioMuebleTramoNivelID);
                await db.ExecuteAsync("Delete from ServicioMuebleProductoNivel where ServicioMuebleTramoNivelID =" + niv.ServicioMuebleTramoNivelID);
                // var tramos = dbsincrona.Table<ServicioMuebleTramoNivelCategoria>().
                //     Where(x => x.ServicioMuebleTramoNivelLocalID == niv.ServicioMuebleTramoNivelLocalID);
                // var productonivel = dbsincrona.Table<ServicioMuebleProductoNivel>().
                //     Where(x => x.ServicioMuebleTramoNivelLocalID == niv.ServicioMuebleTramoNivelLocalID);

                await db.DeleteAsync(niv);
            }*/
        }
        public async Task CargarDatosdemueble() {
            var prueba = await probarred("Carga de Muebles: ");
            if (prueba)
                return;
         
            var m = await db.Table<ServicioMueble>().Where(x => x.Sincronizado == false || x.Sincronizado==null ).ToListAsync();
            if (m.Count > 0)
            {
                
                foreach (var i in m) {
                   // i.DispositivoID = DispositivoID;
                    //i.DispositivoNombre = DispositivoName;
                    await Reportarproceso("Cargando Mueble local" + i.ServicioMuebleLocalID);

                    var r = await repoapi.CargarMuebles(i);
                    if (r.realizado)
                    {
                        try
                        {
                            var mueble = r.MueblesSync.FirstOrDefault();

                            await db.ExecuteAsync
                                ("update ServicioMuebleTramo set ServicioMuebleID=? where ServicioMuebleLocalID=?",
                                mueble.ServicioMuebleID,i.ServicioMuebleLocalID
                                );
                            await db.ExecuteAsync
                                ("update ServicioMuebleImagen set ServicioMuebleID=? where ServicioMuebleLocalID=?",
                                mueble.ServicioMuebleID, i.ServicioMuebleLocalID
                                );
                            await db.ExecuteAsync
                                ("update ServicioMuebleTramoNivel set ServicioMuebleID=? where ServicioMuebleLocalID=?",
                                mueble.ServicioMuebleID, i.ServicioMuebleLocalID
                                );
                            /*
                            var tramos = dbsincrona.Table<ServicioMuebleTramo>().
                            Where(x => x.ServicioMuebleLocalID == i.ServicioMuebleLocalID);
                            var imagenes = dbsincrona.Table<ServicioMuebleImagen>().
                            Where(x => x.ServicioMuebleLocalID == i.ServicioMuebleLocalID);
                            var niveles = dbsincrona.Table<ServicioMuebleTramoNivel>().
                            Where(x => x.ServicioMuebleLocalID == i.ServicioMuebleLocalID);

                            foreach (var t in tramos)
                            {
                                t.ServicioMuebleID = mueble.ServicioMuebleID;
                               await db.UpdateAsync(t);
                            }

                            foreach (var image in imagenes)
                            {
                                image.ServicioMuebleID = mueble.ServicioMuebleID;
                                await db.UpdateAsync(image);
                            }
                            foreach (var n in niveles)
                            {
                                n.ServicioMuebleID = mueble.ServicioMuebleID;
                              await  db.UpdateAsync(n);
                            }*/

                            await db.ExecuteAsync("update ServicioMueble set Sincronizado=1 where ServicioMuebleLocalID=?",i.ServicioMuebleLocalID);
                            /*
                            var tempm = await db.Table<ServicioMueble>().FirstOrDefaultAsync(x=>x.ServicioMuebleLocalID==i.ServicioMuebleLocalID);
                                tempm.Sincronizado = true;
                                await db.UpdateAsync(tempm);*/
                          
                        }
                        catch (Exception ex)
                        {
                            await Reportarproceso("Error en actualizacion de dependencias de mueble: " + ex.Message,
                                true, JsonConvert.SerializeObject(i), "Carga de muebles");

                        }
                    }
                    else {
                        await Reportarproceso("Error Carga mueble " + resultado.Errores, true, JsonConvert.SerializeObject(i), "Carga de muebles");

                    }
                }
            }
            else
            {

            }
        }
        public async Task guardarimagenesdelmuebles(ServicioMueble mueble) {
            var imagenes = await db.Table<ServicioMuebleImagen>().Where(x=>(x.ServicioMuebleID==0 || x.ServicioMuebleID==null) && (x.ServicioMuebleLocalID ==0 )).ToListAsync();

            foreach (var img in imagenes) {
                img.ServicioMuebleLocalID = mueble.ServicioMuebleLocalID??0;
                img.ServicioMuebleID = mueble.ServicioMuebleID;
                dbsincrona.Update(img);
            }
        }


        public async Task<bool> verificarminimodefotos(ServicioMueble parametro) {
            var cantfotos = await db.Table<ServicioMuebleImagen>().Where(x => (x.ServicioMuebleID == 0 || x.ServicioMuebleID == null) && (x.ServicioMuebleLocalID == 0)).ToListAsync();

            if (cantfotos.Count >= (parametro.MuebleTramos + 2))
            {
                return true;
            }
            else {
                await mensajetoast("Debe cargar un minimo de "+(parametro.MuebleTramos + 2)+" fotos para el mueble antes de guardar");
                return false;
            }
        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de Muebles: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando  Muebles " );

                resultfromAPIMueble datos = await repoapi.DescargaMuebles();
                if (datos.realizado)
                {
                    if (datos.Mueble.Count > 0)
                    {
                        await clearData();

                        await insertdata(datos.Mueble, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Muebles " + datos.Errores, true, "", "Descarga de muebles");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Muebles " + ex.Message + ex.StackTrace, true, "", "Descarga de muebles");

                Debug.WriteLine(ex.Message);

                //await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<List<ServicioMueble>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
