using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{
  
    public class ServicioProductoNivelOperaciones : ITableoperations<ServicioMuebleProductoNivel>
    {



        public override async Task<resultfromLocalDB<ServicioMuebleProductoNivel>> consultarDatoconcisoAsync(ServicioMuebleProductoNivel parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<ServicioMuebleProductoNivel>().FirstOrDefaultAsync();
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

        public override async Task insertarregistro(ServicioMuebleProductoNivel parameter)
        {
            await Reportarproceso("Guardando producto Nivel " + parameter.ServicioMuebleProductoNivelID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            parameter.DispositivoID = DispositivoID;

            await base.insertarregistro(parameter);

        }

        public async Task CargarDatos()
        {
            var prueba = await probarred("Carga de Producto nivel: ");
            if (prueba)
                return;
            var m = await db.Table<ServicioMuebleProductoNivel>().Where(x =>
            (x.Sincronizado == false || x.Sincronizado == null)&&
            (x.ProductoID!=0&&x.ProductoID!=null)&&
            (x.ServicioMuebleTramoNivelCategoriaID!=0&&x.ServicioMuebleTramoNivelCategoriaID!=null)&&
            (x.ServicioMuebleTramoNivelID!=0&&x.ServicioMuebleTramoNivelID!=null)).ToListAsync();

            if (m.Count > 0)
            {
                foreach (var i in m)
                {
                    var nivel = await db.Table<ServicioMuebleTramoNivel>().FirstOrDefaultAsync(
                        x=>(x.ServicioMuebleTramoNivelID==i.ServicioMuebleTramoNivelID && i.ServicioMuebleTramoNivelID!=0) ||
                        (x.ServicioMuebleTramoNivelLocalID == i.ServicioMuebleTramoNivelLocalID && i.ServicioMuebleTramoNivelLocalID!=0));
                    i.ServicioID = nivel.ServicioID;
                   // i.DispositivoID = DispositivoID;
                   // i.DispositivoNombre = DispositivoName;
                  //  i.FechaHoraLocal = DateTime.Now;
                    await Reportarproceso("Carga de Servicio Producto Nivel local " + i.ServicioMuebleProductoNivelLocalID);

                    var r = await repoapi.CargaProductoNivelPOST(i);
                    if (r.realizado)
                    {
                        try
                        {
                            await db.ExecuteAsync
                                ("update ServicioMuebleProductoNivel set Sincronizado=1 where ServicioMuebleProductoNivelLocalID=?",i.ServicioMuebleProductoNivelLocalID);
                            /*
                            var tempm = await db.Table<ServicioMuebleProductoNivel>().FirstOrDefaultAsync(x => x.ServicioMuebleProductoNivelLocalID == i.ServicioMuebleProductoNivelLocalID);
                            tempm.Sincronizado = true;
                            await db.UpdateAsync(tempm);*/
                        }
                        catch(Exception ex) {
                            await Reportarproceso("Error al actualizar de Servicio Producto Nivel local sincronizado" + ex.Message+ex.StackTrace,
                                true, JsonConvert.SerializeObject(i), "Carga de producto nivel");
                        }
                    }
                    else {
                        await Reportarproceso("Error de Carga de Servicio Producto Nivel local " + resultado.Errores, true, JsonConvert.SerializeObject(i), "Carga de producto nivel");

                    }
                }
            }
            else
            {

            }
        }


        public async Task<resultfromLocalDB<List<ServicioMuebleProductoNivel>>> Listadenivelesproducto(ServicioMuebleTramo parameter,Producto upc)
        {
            resultfromLocalDB<List<ServicioMuebleProductoNivel>> res = new resultfromLocalDB<List<ServicioMuebleProductoNivel>>() { realizado = true, Errores = "" };

            try
            {

                var producto = await db.Table<Producto>().FirstOrDefaultAsync(x => x.UPC == upc.UPC);
                var niveles = await db.Table<ServicioMuebleTramoNivel>().Where(y=>
                (y.ServicioMuebleTramoID ==parameter.ServicioMuebleTramoID && parameter.ServicioMuebleTramoID!=null && parameter.ServicioMuebleTramoID!=0) ||
                (y.ServicioMuebleTramoLocalID ==parameter.ServicioMuebleTramoLocalID && parameter.ServicioMuebleTramoLocalID!=0) ).ToListAsync();
                List<ServicioMuebleProductoNivel> m = new List<ServicioMuebleProductoNivel>();

                foreach (var n in niveles) {
                    var temop =await  db.Table<ServicioMuebleProductoNivel>().Where(x=>
                    (
                    (x.ServicioMuebleTramoNivelID ==n.ServicioMuebleTramoNivelID && n.ServicioMuebleTramoNivelID!=0)||
                    (x.ServicioMuebleTramoNivelLocalID==n.ServicioMuebleTramoNivelLocalID &&
                    n.ServicioMuebleTramoNivelLocalID!=0 && n.ServicioMuebleTramoNivelLocalID!=null)) &&
                     ((x.ProductoID == producto.ProductoID && producto.ProductoID!=0 && x.ProductoLocalID==0) || (
                    x.ProductoLocalID== producto.ProductoLocalID && producto.ProductoLocalID!=0))
                    ).ToListAsync();
                    m.AddRange(temop);

                }
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
                await db.ExecuteAsync("Delete from ServicioMuebleProductoNivel where Sincronizado = 1");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }



        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de producto nivel: ");
            if (prueba)
                return;
            //  await clearData();
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando producto Nivel ");
                resultfromAPIServicioMuebleProductoNivel datos = await repoapi.DescargaServicioMuebleProductoNivel();
                if (datos.realizado)
                {
                    if (datos.ProductoNivel.Count > 0)
                    {
                        await clearData();
                        await insertdata(datos.ProductoNivel, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Producto Nivel " + datos.Errores, true, "", "Descarga de producto nivel");
                }
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Producto Nivel "+ex.Message + ex.StackTrace,
                    true,"" , "Descarga de producto nivel");

                Debug.WriteLine(ex.Message);

            }

        }

        public override async Task<resultfromLocalDB<List<ServicioMuebleProductoNivel>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<ServicioMuebleProductoNivel>> res = new resultfromLocalDB<List<ServicioMuebleProductoNivel>>() { realizado = true, Errores = "" };

            try
            {
                ServicioMuebleTramo tr = (ServicioMuebleTramo)parameter;

                var servicios = dbsincrona.Table<ServicioMuebleTramoNivel>().Where(x=>(x.ServicioMuebleTramoID == tr.ServicioMuebleTramoID && tr.ServicioMuebleTramoID!=null && tr.ServicioMuebleTramoID!=0)||
                (x.ServicioMuebleTramoLocalID == tr.ServicioMuebleTramoLocalID && tr.ServicioMuebleTramoLocalID!=0)).ToList();
                List<ServicioMuebleProductoNivel> m = new List<ServicioMuebleProductoNivel>();

                foreach (var s in servicios) {
                    var found = dbsincrona.Table<ServicioMuebleProductoNivel>().Where(x=>((x.ServicioMuebleTramoNivelID==s.ServicioMuebleTramoNivelID && s.ServicioMuebleTramoNivelID!=0) ||
                    (x.ServicioMuebleTramoNivelLocalID == s.ServicioMuebleTramoNivelLocalID && s.ServicioMuebleTramoNivelLocalID!=null && s.ServicioMuebleTramoNivelLocalID!=0))
                    && ((x.CategoriaID ==tr.Categoria.CategoriaID && tr.Categoria.CategoriaID!=null && tr.Categoria.CategoriaID!=0) ||
                    ((x.CategoriaLocalID == tr.Categoria.CategoriaLocalID && tr.Categoria.CategoriaLocalID != null && tr.Categoria.CategoriaLocalID != 0))
                    )).ToList();

                    m.AddRange(found);
                }
                foreach(var f in m){
                    f.producto = dbsincrona.Table<Producto>().FirstOrDefault(x=>(x.ProductoID == f.ProductoID && f.ProductoID!=0 && f.ProductoID!=null)
                    ||( x.ProductoLocalID == f.ProductoLocalID && f.ProductoLocalID!=0));
                    f.tramo = tr;
                }

                if (m.Count > 0)
                {
                    m = m.Where(x=>x.producto!=null).ToList();
                    m = m.GroupBy(x=>x.producto.UPC).Select(pr=>pr.FirstOrDefault()).OrderByDescending(x=>x.ServicioMuebleProductoNivelLocalID).ToList();
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
                Debug.WriteLine(ex.Message);
                return res;
            }
        }


        public async Task guardarregistroproductonivel(ServicioMuebleProductoNivel productonivel) {
            try
            {
                ServicioMuebleProductoNivel preexistentenivel = await db.Table<ServicioMuebleProductoNivel>().FirstOrDefaultAsync(x => x.ServicioMuebleProductoNivelLocalID == productonivel.ServicioMuebleProductoNivelLocalID);
                if (preexistentenivel == null)
                {

                    var nivelcategoria = await db.Table<ServicioMuebleTramoNivelCategoria>().FirstOrDefaultAsync(x =>
                    ((x.ServicioMuebleTramoNivelID == productonivel.ServicioMuebleTramoNivelID && productonivel.ServicioMuebleTramoNivelID != 0) ||
                    (x.ServicioMuebleTramoNivelLocalID == productonivel.ServicioMuebleTramoNivelLocalID && productonivel.ServicioMuebleTramoNivelLocalID != 0)) &&
                    ((x.CategoriaID == productonivel.CategoriaID && productonivel.CategoriaID != 0 && productonivel.CategoriaID != 0) ||
                    (x.CategoriaLocalID == productonivel.CategoriaLocalID && productonivel.CategoriaLocalID != 0))
                    );

                    productonivel.ServicioMuebleTramoNivelCategoriaID = nivelcategoria.ServicioMuebleTramoNivelCategoriaID;
                    productonivel.ServicioMuebleTramoNivelCategoriaLocalID = nivelcategoria.ServicioMuebleTramoNivelCategoriaLocalID ?? 0;

                    preexistentenivel = await db.Table<ServicioMuebleProductoNivel>().FirstOrDefaultAsync(x =>
                               ((x.ServicioMuebleTramoNivelID == productonivel.ServicioMuebleTramoNivelID && productonivel.ServicioMuebleTramoNivelID != 0) ||
                   (x.ServicioMuebleTramoNivelLocalID == productonivel.ServicioMuebleTramoNivelLocalID && productonivel.ServicioMuebleTramoNivelLocalID != 0)) &&
                  ((x.ServicioMuebleTramoNivelCategoriaID == productonivel.ServicioMuebleTramoNivelCategoriaID && productonivel.ServicioMuebleTramoNivelCategoriaID != 0 && productonivel.ServicioMuebleTramoNivelCategoriaID != null) ||
                  (x.ServicioMuebleTramoNivelCategoriaLocalID == productonivel.ServicioMuebleTramoNivelCategoriaLocalID && productonivel.ServicioMuebleTramoNivelCategoriaLocalID != 0)) && (x.ProductoID == productonivel.ProductoID && x.ProductoID != 0 && x.ProductoID != null)
                  && (x.ProductoLocalID == productonivel.ProductoLocalID && productonivel.ProductoLocalID != 0)
                   );

                }

                if (preexistentenivel == null)
                {
                    productonivel.Sincronizado = false;
                    await insertarregistro(productonivel);
                }
                else
                {

                    await db.ExecuteAsync
                        ("update ServicioMuebleProductoNivel set Frente=?,Profundo=?,Posicion=? where ServicioMuebleProductoNivelLocalID=?",
                        productonivel.Frente, productonivel.Profundo, productonivel.Posicion, preexistentenivel.ServicioMuebleProductoNivelLocalID);

                }
            }
            catch(Exception ex) {
             await   escribirerror(ex.Message,"GuardarProductoNivel","guardado de nivel", JsonConvert.SerializeObject(productonivel),ex.StackTrace);
               await mensajetoast("Error guardado nivel: "+ex.Message);
            }

        }


    }
}
