using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{
   

    public class MuebleTramoNivelCategoriaOperaciones : ITableoperations<ServicioMuebleTramoNivelCategoria>
    {



        public async Task<resultfromLocalDB<List<ServicioMuebleTramoNivelCategoria>>> ListademuebleTramoNivelCategoria(ServicioMuebleTramo parameter)
        {
            resultfromLocalDB<List<ServicioMuebleTramoNivelCategoria>> res = new resultfromLocalDB<List<ServicioMuebleTramoNivelCategoria>>() { realizado = true, Errores = "" };

            try
            {

                var m = await db.Table<ServicioMuebleTramoNivelCategoria>().Where(x =>
                x.ServicioMuebleTramoID == parameter.ServicioMuebleTramoID).ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay categorias para este mueble";
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
                await db.ExecuteAsync("Delete from ServicioMuebleTramoNivelCategoria where Sincronizado=1");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        public override async Task insertarregistro(ServicioMuebleTramoNivelCategoria parameter)
        {
            await Reportarproceso("Guardando Tramo Nivel Categoria " + parameter.ServicioMuebleTramoNivelCategoriaID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            parameter.DispositivoID = DispositivoID;

            await base.insertarregistro(parameter);

        }


        public async Task CargarDatos()
        {
            var prueba = await probarred("Carga de categorias de nivel: ");
            if (prueba)
                return;
            var m = await db.Table<ServicioMuebleTramoNivelCategoria>().
                Where(x => (x.Sincronizado == false) &&(x.ServicioMuebleTramoID!=0 && x.ServicioMuebleTramoID!=null)&&(x.ServicioMuebleTramoNivelID!=0 && x.ServicioMuebleTramoNivelID!=null)).ToListAsync();
            if (m.Count > 0)
            {
                foreach (var i in m)
                {
                    var tramo = await db.Table<ServicioMuebleTramoNivel>().FirstOrDefaultAsync(
                        x=>x.ServicioMuebleTramoNivelLocalID==i.ServicioMuebleTramoNivelLocalID ||
                        (x.ServicioMuebleTramoNivelID ==i.ServicioMuebleTramoNivelID && i.ServicioMuebleTramoNivelID!=null && i.ServicioMuebleTramoNivelID!=0));
                   
                    var tramoservicio =await db.Table<ServicioMuebleTramo>().FirstOrDefaultAsync(
                        x => x.ServicioMuebleTramoLocalID == tramo.ServicioMuebleTramoLocalID ||
                        (x.ServicioMuebleTramoID == i.ServicioMuebleTramoID && i.ServicioMuebleTramoID != null && i.ServicioMuebleTramoID != 0)
                        );
                    if (tramoservicio == null)
                    {
                        i.ServicioID = tramo.ServicioID;
                    }
                    else
                    {
                        i.ServicioID = tramoservicio.ServicioID;
                    }
                   // i.DispositivoID = DispositivoID;
                   // i.DispositivoNombre = DispositivoName;
                    //i.FechaHoraLocal = DateTime.Now;
                  
                    await Reportarproceso("Cargando TramoNivel Categoria local " + i.ServicioMuebleTramoNivelCategoriaLocalID);

                    var r = await repoapi.CargaMuebleTramoNivelCategoriaPOST(i);
                    if (r.realizado)
                    {
                        var serverid = r.TramoNivelCategoriasSync.FirstOrDefault();

                        await db.ExecuteAsync
                            ("update ServicioMuebleProductoNivel set ServicioMuebleTramoNivelCategoriaID=? where ServicioMuebleTramoNivelCategoriaLocalID=?",
                            serverid.ServicioMuebleTramoNivelCategoriaID,i.ServicioMuebleTramoNivelCategoriaLocalID);
                        /*
                        var productonivel = dbsincrona.Table<ServicioMuebleProductoNivel>().
                            Where(x => x.ServicioMuebleTramoNivelCategoriaLocalID == i.ServicioMuebleTramoNivelCategoriaLocalID);
                            foreach (var pn in productonivel) {
                                pn.ServicioMuebleTramoNivelCategoriaID = serverid.ServicioMuebleTramoNivelCategoriaID;
                              await  db.UpdateAsync(pn);
                            }
                            */
                        try
                        {
                            await db.ExecuteAsync
                                ("update ServicioMuebleTramoNivelCategoria set Sincronizado=1 where ServicioMuebleTramoNivelCategoriaLocalID=?",
                                i.ServicioMuebleTramoNivelCategoriaLocalID);
                            /*
                            var tempm = await db.Table<ServicioMuebleTramoNivelCategoria>().
                                FirstOrDefaultAsync(
                                x => x.ServicioMuebleTramoNivelCategoriaLocalID == i.ServicioMuebleTramoNivelCategoriaLocalID);
                            tempm.Sincronizado = true;
                            await db.UpdateAsync(tempm);*/
                        }
                        catch(Exception) {
                            await Reportarproceso("Error al actualizar Tramo Nivel Categoria sincronizado " + r.Errores,
                                true, i.ServicioMuebleTramoNivelCategoriaLocalID, "Carga de tramo nivel categoria");

                        }
                        // await db.InsertOrReplaceAsync(r.TramosSync.FirstOrDefault());

                    }
                    else {
                        await Reportarproceso("Error Cargando Tramo Nivel Categoria " + r.Errores,
                            true, i.ServicioMuebleTramoNivelCategoriaLocalID, "Carga de tramo nivel categoria");

                    }
                }
            }
            else
            {

            }
        }


        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de tramo nivel categoria: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Tramo Nivel Categoria " );

                resultfromAPIMuebleTramosNivelCategoria datos = await repoapi.DescargaMuebleTramoNivelCategoria();
                if (datos.realizado)
                {
                    if (datos.MuebleTramoNivelCategoria.Count > 0)
                    {
                        await clearData();

                        await insertdata(datos.MuebleTramoNivelCategoria, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Tramo Nivel Categoria " + datos.Errores,
                        true, "", "Descarga de tramo nivel categoria");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Tramo NIvel Categoria " + ex.Message+ex.StackTrace, true, "", "Descarga de tramos nivel categoria");

                Debug.WriteLine(ex.Message);

              //  await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<ServicioMuebleTramoNivelCategoria>> consultarDatoconcisoAsync(ServicioMuebleTramoNivelCategoria parameter)
        {
            throw new NotImplementedException();
        }

        public override Task<resultfromLocalDB<List<ServicioMuebleTramoNivelCategoria>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
