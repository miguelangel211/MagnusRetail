using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;

namespace CheckstoresMagnusRetail.sqlrepo
{

    public class CategoriasOperaciones : ITableoperations<Categoria>
    {



        public async Task<resultfromLocalDB<List<Categoria>>> ListadeCategorias()
        {
            resultfromLocalDB<List<Categoria>> res = new resultfromLocalDB<List<Categoria>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<Categoria>().ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m.OrderBy(x=>x.CategoriaNombre).ToList();
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay Categorias";
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

        public async Task<resultfromLocalDB<List<Categoria>>> ListadeCategoriasdeltramo()
        {
            resultfromLocalDB<List<Categoria>> res = new resultfromLocalDB<List<Categoria>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<Categoria>().ToListAsync();

                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay Categorias";
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


        public async Task<resultfromLocalDB<List<Categoria>>> ListadeCategoriasdelmueble(ServicioMuebleTramo Parametro)
        {
            resultfromLocalDB<List<Categoria>> res = new resultfromLocalDB<List<Categoria>>() { realizado = true, Errores = "" };

            try
            {
                var tramonivel =await db.Table<ServicioMuebleTramoNivel>().FirstOrDefaultAsync(x=>
                (x.ServicioMuebleTramoID == Parametro.ServicioMuebleTramoID && Parametro.ServicioMuebleTramoID!=null && Parametro.ServicioMuebleTramoID!=0)||
                                (x.ServicioMuebleTramoLocalID == Parametro.ServicioMuebleTramoLocalID  && Parametro.ServicioMuebleTramoLocalID != 0) 

                );
                  var n =  dbsincrona.Table<ServicioMuebleTramoNivelCategoria>().
                    Where(x=>(x.ServicioMuebleTramoNivelID == tramonivel.ServicioMuebleTramoNivelID && tramonivel.ServicioMuebleTramoNivelID != 0)||
                    (x.ServicioMuebleTramoNivelLocalID == tramonivel.ServicioMuebleTramoNivelLocalID && tramonivel.ServicioMuebleTramoNivelLocalID != 0)
                    ).Select(k => k.CategoriaID).ToList();
               
                //  var m = dbsincrona.Table<resultfromAPIMuebleTramosNivelCategoria>().ToList();
                var m = await db.Table<Categoria>().Where(x=>n.Contains(x.CategoriaID)).OrderBy(x=>x.CategoriaNombre).ToListAsync();
                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No hay Categorias";
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


        public async Task<resultfromLocalDB<List<Categoria>>> ListadeCategoriasdelmuebleformaseleccionados(ServicioMueble Parametro)
        {
            var tramos = await db.Table<ServicioMuebleTramo>().Where(x =>( x.ServicioMuebleID == Parametro.ServicioMuebleID && Parametro.ServicioMuebleID!=0 && Parametro.ServicioMuebleID!=null)||
            (x.ServicioMuebleLocalID==Parametro.ServicioMuebleLocalID && Parametro.ServicioMuebleLocalID != 0 && Parametro.ServicioMuebleLocalID != null)).ToListAsync();
            if (tramos.Count > 0)
            {
                return await ListadeCategoriasdelmueble(tramos.FirstOrDefault());
            }
            else {
                return new resultfromLocalDB<List<Categoria>>() { realizado = false, Errores = "no hay datos" };
            }
        }
        public override async Task clearData()
        {
            try
            {
                 dbsincrona.Execute("Delete from Categoria");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        public override async Task insertarregistro(Categoria parameter)
        {
            await Reportarproceso("Guardando categoria " + parameter.CategoriaID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            await base.insertarregistro(parameter);

        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de Categorias: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Categorias ");

                resultfromAPICategorias datos = await repoapi.DescargaCategorias();
                if (datos.realizado)
                {
                    await clearData();
                    await Task.Delay(500);

                    if (datos.Categorias.Count > 0)
                    {
                        await insertdata(datos.Categorias, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar categorias " + datos.Errores, true,"", "Descarga de categorias");

                    // await mensajetoast(datos.Errores);
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar categorias " + ex.Message + ex.StackTrace, true, "", "Descarga de categorias");

                Debug.WriteLine(ex.Message);

               // await mensajetoast(ex.StackTrace);
            }

        }

        public override Task<resultfromLocalDB<Categoria>> consultarDatoconcisoAsync(Categoria parameter)
        {
            throw new NotImplementedException();
        }

        public override Task<resultfromLocalDB<List<Categoria>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
