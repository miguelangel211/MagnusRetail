using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{


    public class TramosOperaciones : ITableoperations<ServicioMuebleTramo>
    {



        public async Task<resultfromLocalDB<List<ServicioMuebleTramo>>> ListadeTRamos(ServicioMueble parameter)
        {
            resultfromLocalDB<List<ServicioMuebleTramo>> res = new resultfromLocalDB<List<ServicioMuebleTramo>>() { realizado = true, Errores = "" };

            try
            {
                
                   var m = await db.Table<ServicioMuebleTramo>().Where(x =>
                   (x.ServicioMuebleID == parameter.ServicioMuebleID && parameter.ServicioMuebleID!=0 && parameter.ServicioMuebleID!=null)||
                   (x.ServicioMuebleLocalID==parameter.ServicioMuebleLocalID && parameter.ServicioMuebleLocalID != 0 && parameter.ServicioMuebleLocalID != null)).ToListAsync();

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
              await   db.ExecuteAsync("Delete from ServicioMuebleTramo where Sincronizado=1");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
        public override async Task insertarregistro(ServicioMuebleTramo parameter)
        {
            await Reportarproceso("Guardando Tramo " + parameter.ServicioMuebleTramoID,false,null,null);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            parameter.DispositivoID = DispositivoID;
            await base.insertarregistro(parameter);

        }

        public async Task CargarDatos()
        {
            var prueba = await probarred("Carga de tramos");
            if (prueba)
                return;
            var m = await db.Table<ServicioMuebleTramo>().Where(x => (x.Sincronizado == false)&&(x.ServicioMuebleID!=0&&x.ServicioMuebleID!=null)).ToListAsync();
            if (m.Count > 0)
            {
                foreach (var i in m)
                {
                    await Reportarproceso("Cargando Tramo local " + i.ServicioMuebleTramoLocalID);

                  //  i.DispositivoID = DispositivoID;
                  //  i.DispositivoNombre = DispositivoName;
                    var r = await repoapi.CargarTramosPOST(i);
                    if (r.realizado)
                    {
                        try
                        {
                            var tramo = r.TramosSync.FirstOrDefault();
                            
                            await db.ExecuteAsync
                                ("update ServicioMuebleTramoNivel set ServicioMuebleTramoID =? where ServicioMuebleTramoLocalID=?", tramo.ServicioMuebleTramoID,i.ServicioMuebleTramoLocalID);
                            await db.ExecuteAsync
                                ("update ServicioMuebleTramoNivelCategoria set ServicioMuebleTramoID =? where ServicioMuebleTramoLocalID=?",
                                tramo.ServicioMuebleTramoID, i.ServicioMuebleTramoLocalID);
                            /*
                            var niveles =
                            dbsincrona.Table<ServicioMuebleTramoNivel>().
                            Where(x => x.ServicioMuebleTramoLocalID == i.ServicioMuebleTramoLocalID);
                            var categorias =await db.Table<ServicioMuebleTramoNivelCategoria>().
                            Where(x => x.ServicioMuebleTramoLocalID == i.ServicioMuebleTramoLocalID).ToListAsync();

                            foreach (var n in niveles)
                            {
                                n.ServicioMuebleTramoID = tramo.ServicioMuebleTramoID;
                               await db.UpdateAsync(n);
                            }

                            foreach (var cat in categorias)
                            {
                                cat.ServicioMuebleTramoID = tramo.ServicioMuebleTramoID;
                              await  db.UpdateAsync(cat);
                            }*/
                            try
                            { 
                                await db.ExecuteAsync("update ServicioMuebleTramo set Sincronizado=1 where ServicioMuebleTramoLocalID=?",
                                    i.ServicioMuebleTramoLocalID);
                               /* var tempm =await db.Table<ServicioMuebleTramo>().FirstOrDefaultAsync(x => x.ServicioMuebleTramoLocalID == i.ServicioMuebleTramoLocalID);
                                tempm.Sincronizado = true;
                                await db.UpdateAsync(tempm);*/
                            }
                            catch(Exception ex) {
                                await Reportarproceso("Error actualizacion  de tramo sincronizado " + ex.Message +ex.StackTrace
                                    ,true,JsonConvert.SerializeObject(i),"Carga de tramos"
                                    );

                            }

                        }
                        catch (Exception ex)
                        {
                            await Reportarproceso("Error actualizacion dependencias de tramo " + ex.Message,
                                true, JsonConvert.SerializeObject(i), "Carga de tramos");

                        }
                    }
                    else {

                        await Reportarproceso("Error en carga de Tramo " + resultado.Errores, true, JsonConvert.SerializeObject(i), "Carga de tramos");
                    }
                }
            }
            else
            {

            }
        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de tramos: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Tramos ");

                resultfromAPITramos datos = await repoapi.DescargaTramos();
                if (datos.realizado)
                {

                    if (datos.Tramo.Count > 0)
                    {
                      //  await clearData();

                        await insertdata(datos.Tramo, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Tramos " + datos.Errores, true,"", "Descarga de tramos");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Tramos " + ex.Message + ex.StackTrace,true, "", "Descarga de tramos");

                Debug.WriteLine(ex.Message);

              //  await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<ServicioMuebleTramo>> consultarDatoconcisoAsync(ServicioMuebleTramo parameter)
        {
            throw new NotImplementedException();
        }

        public override async Task<resultfromLocalDB<List<ServicioMuebleTramo>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<ServicioMuebleTramo>> res = new resultfromLocalDB<List<ServicioMuebleTramo>>() { realizado = true, Errores = "" };
            ServicioMueble parametrotype = (parameter as ServicioMueble);
            try
            {

                var m = await db.Table<ServicioMuebleTramo>().Where(x => (x.ServicioMuebleID == parametrotype.ServicioMuebleID) || x.ServicioMuebleLocalID == parametrotype.ServicioMuebleLocalID).ToListAsync();

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
                Debug.WriteLine(ex.Message);
                return res;
            }
        }
    }
}
