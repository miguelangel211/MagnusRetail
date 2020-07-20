using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{

    public class TramoNivelesOperaciones : ITableoperations<ServicioMuebleTramoNivel>
    {



        public async Task<resultfromLocalDB<List<ServicioMuebleTramoNivel>>> Listadenivelestramo(ServicioMuebleTramo parameter)
        {
            resultfromLocalDB<List<ServicioMuebleTramoNivel>> res = new resultfromLocalDB<List<ServicioMuebleTramoNivel>>() { realizado = true, Errores = "" };

            try
            {

                var m = await db.Table<ServicioMuebleTramoNivel>().Where(x => (
                x.ServicioMuebleTramoID == parameter.ServicioMuebleTramoID && parameter.ServicioMuebleTramoID!=null && parameter.ServicioMuebleTramoID!=0)||
                (x.ServicioMuebleTramoLocalID ==parameter.ServicioMuebleTramoLocalID
                && parameter.ServicioMuebleTramoLocalID!=0)).ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay tramos para este mueble";
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
                await db.ExecuteAsync("Delete from ServicioMuebleTramoNivel where Sincronizado=1");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
        public override async Task insertarregistro(ServicioMuebleTramoNivel parameter)
        {
            await Reportarproceso("Guardando Nivel " + parameter.ServicioMuebleTramoNivelID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            parameter.DispositivoID = DispositivoID;
            await base.insertarregistro(parameter);

        }

        public async Task CargarDatos()
        {
            var prueba = await probarred("Carga de Niveles de Tramo");
            if (prueba)
                return;
            var m = await db.Table<ServicioMuebleTramoNivel>().Where(x => (x.Sincronizado == false || x.Sincronizado==null)&&
            (x.ServicioMuebleID!=0 && x.ServicioMuebleID!=null)&&(x.ServicioMuebleTramoID!=0&&x.ServicioMuebleTramoID!=null)).ToListAsync();
            
            if (m.Count > 0)
            {
                foreach (var i in m)
                {
                    var tramo = await db.Table<ServicioMuebleTramo>().FirstOrDefaultAsync(
                        x=>(x.ServicioMuebleTramoID == i.ServicioMuebleTramoID && i.ServicioMuebleTramoID!=0) ||
                        (x.ServicioMuebleTramoLocalID==i.ServicioMuebleTramoLocalID && i.ServicioMuebleTramoLocalID!=0));
                    i.ServicioID = tramo.ServicioID;
                   // i.DispositivoID = DispositivoID;
                   // i.DispositivoNombre = DispositivoName;
                    await Reportarproceso("Cargando TramoNivel Local " + i.ServicioMuebleTramoLocalID);

                    var r = await repoapi.CargarTramosNivelPOST(i);
                    if (r.realizado)
                    {
                        try
                        {
                            var mueble = r.TramoNivelsSync.FirstOrDefault();
                            await
                                db.ExecuteAsync
                                ("update ServicioMuebleTramoNivelCategoria set ServicioMuebleTramoNivelID=? where ServicioMuebleTramoNivelLocalID=?",
                                mueble.ServicioMuebleTramoNivelID,i.ServicioMuebleTramoNivelLocalID);
                            await
                            db.ExecuteAsync
                            ("update ServicioMuebleProductoNivel set ServicioMuebleTramoNivelID=?  where ServicioMuebleTramoNivelLocalID=?",
                            mueble.ServicioMuebleTramoNivelID, i.ServicioMuebleTramoNivelLocalID);
                            /*
                            var tramos = dbsincrona.Table<ServicioMuebleTramoNivelCategoria>().Where(x => x.ServicioMuebleTramoNivelLocalID == i.ServicioMuebleTramoNivelLocalID);
                            var productonivel = dbsincrona.Table<ServicioMuebleProductoNivel>().Where(x => x.ServicioMuebleTramoNivelLocalID == i.ServicioMuebleTramoNivelLocalID);

                            foreach (var t in tramos)
                            {
                                t.ServicioMuebleTramoNivelID = mueble.ServicioMuebleTramoNivelID;
                                //t.ServicioMuebleTramoID = mueble.ServicioMuebleTramoID;
                              await  db.UpdateAsync(t);
                            }
                            foreach (var p in productonivel)
                            {
                                p.ServicioMuebleTramoNivelID = mueble.ServicioMuebleTramoNivelID;
                              await  db.UpdateAsync(p);
                            }
                            */
                            try
                            {
                                await db.ExecuteAsync("update ServicioMuebleTramoNivel set Sincronizado=1 where ServicioMuebleTramoNivelLocalID=?",i.ServicioMuebleTramoNivelLocalID);
                                /*
                                var tempm = await db.Table<ServicioMuebleTramoNivel>().FirstOrDefaultAsync(
                                x => x.ServicioMuebleTramoNivelLocalID == i.ServicioMuebleTramoNivelLocalID);
                                tempm.Sincronizado = true;
                                await db.UpdateAsync(tempm);*/
                            }
                            catch(Exception ex) {
                                await Reportarproceso("Erroren en actualizacion de dependencias de Nivel sincronizado"
                                    + ex.Message+ex.StackTrace, true, JsonConvert.SerializeObject(i), "Carga de niveles de tramos");
                            }
                        }
                        catch (Exception ex)
                        {
                            await Reportarproceso("Erroren en actualizacion de dependencias de Nivel" + ex.Message, true, JsonConvert.SerializeObject(i), "Carga de niveles de tramos");

                        }
                    }
                    else {
                        await Reportarproceso("Error en carga de Tramo Nivel " + resultado.Errores, true, JsonConvert.SerializeObject(i), "Carga de niveles de tramos");

                    }
                }
            }
            else
            {

            }
        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de niveles: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Niveles ");

                resultfromAPITramosNivel datos = await repoapi.DescargaTramosNivel();
                if (datos.realizado)
                {

                    if (datos.TramoNivel.Count > 0)
                    {
                        await clearData();

                        await insertdata(datos.TramoNivel, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar  Niveles " + datos.Errores, true, "", "Descarga de niveles de tramos");

                    //await mensajetoast(datos.Errores);
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar  Niveles " + ex.Message + ex.StackTrace, true, "", "Descarga de niveles de tramos");

                Debug.WriteLine(ex.Message);

               // await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<ServicioMuebleTramoNivel>> consultarDatoconcisoAsync(ServicioMuebleTramoNivel parameter)
        {
            throw new NotImplementedException();
        }

        public override async Task<resultfromLocalDB<List<ServicioMuebleTramoNivel>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<ServicioMuebleTramoNivel>> res = new resultfromLocalDB<List<ServicioMuebleTramoNivel>>() { realizado = true, Errores = "" };

            try
            {
                ServicioMueble mueble = (ServicioMueble)parameter;
                var m = await db.Table<ServicioMuebleTramoNivel>().Where(x => (x.ServicioMuebleID == mueble.ServicioMuebleID && mueble.ServicioMuebleID != 0 && mueble.ServicioMuebleID != null) ||
                x.ServicioMuebleLocalID == mueble.ServicioMuebleLocalID && mueble.ServicioMuebleLocalID != 0 && mueble.ServicioMuebleLocalID != null).ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay tramos para este mueble";
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
    }
}
