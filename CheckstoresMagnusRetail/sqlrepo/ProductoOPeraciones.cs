using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{
    public class ProductoOperacion : ITableoperations<Producto>
    {



        public override async Task<resultfromLocalDB<Producto>> consultarDatoconcisoAsync(Producto parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<Producto>().FirstOrDefaultAsync(x => x.UPC == parameter.UPC);
                if (resultado.Result != null)
                {
                    resultado.Result.Categoria =await db.Table<Categoria>().FirstOrDefaultAsync(x=>x.CategoriaID == resultado.Result.CategoriaID);
                    resultado.realizado = true;
                }
                else
                {
                    resultado.realizado = false;
                }
                return resultado;
            }
            catch {
                resultado.realizado = false;
                return resultado;
            }
        }

        public async Task guardarimagenesdelproducto(Producto prod)
        {
            try
            {
                var data = await db.QueryAsync<ProductoImagen>("select * from ProductoImagen where ProductoID is null and ProductoLocalID is null");
                await db.ExecuteAsync
                    ("update ProductoImagen set ProductoLocalID=?," +
                    "ProductoID=?  where ProductoID is null and ProductoLocalID is null",
                    prod.ProductoLocalID, prod.ProductoID);
            }
            catch(Exception ex) {
              await  escribirerror(ex.Message,"actualizar imagenes para producto","Guardar imagenes de producto","",ex.StackTrace);
            }

        }
        public  async Task insertarregistro(Producto parameter,bool fuente)
        {
            if(fuente)
                await Reportarproceso("Guardando Producto " + parameter.ProductoID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;
            parameter.DispositivoID = DispositivoID;

            await base.insertarregistro(parameter);

        }

 

        public async Task CargarDatos()
        {
            var prueba = await probarred("Carga de productos: ");
            if (prueba)
                return;
            var m = await db.Table<Producto>().Where(x =>( x.Sincronizado == false || x.Sincronizado==null) ).ToListAsync();
            if (m.Count > 0)
            {
                foreach (var i in m)
                {
                    i.DispositivoID = DispositivoID;
                    i.DispositivoNombre = DispositivoName;
                    i.UsuarioLocalID = UsuarioID;
                    await Reportarproceso("Carga de producto local " + i.ProductoLocalID);

                    var r = await repoapi.CargaProductoPOST(i);
                    if (r.realizado)
                    {
                        try
                        {var serverid= r.ProductoNuevosSync.FirstOrDefault().ProductoID; 
                            await db.ExecuteAsync
                                ("update ServicioMuebleProductoNivel set ProductoID=? where ProductoLocalID=?",
                                serverid,i.ProductoLocalID);
                            await db.ExecuteAsync
                                ("update ProductoImagen set ProductoID=? where ProductoLocalID=?",
                                serverid, i.ProductoLocalID);
                            /*
                            var productonivel = dbsincrona.Table<ServicioMuebleProductoNivel>().
                            Where(x => x.ProductoLocalID == i.ProductoLocalID);
                            var fotos = dbsincrona.Table<ProductoImagen>().
                            Where(x => x.ProductoLocalID == i.ProductoLocalID);
                            foreach (var n in productonivel)
                            {
                                n.ProductoID = r.ProductoNuevosSync.FirstOrDefault().ProductoID;
                               await db.UpdateAsync(n);
                            }
                            foreach (var n in fotos)
                            {
                                n.ProductoID = r.ProductoNuevosSync.FirstOrDefault().ProductoID;
                              await  db.UpdateAsync(n);
                            }*/
                            try
                            {
                                await db.ExecuteAsync("update Producto set Sincronizado=1 where ProductoLocalID=?",i.ProductoLocalID);
                                /*
                                var tempm = await db.Table<Producto>().FirstOrDefaultAsync(x => x.ProductoLocalID == i.ProductoLocalID);
                                tempm.Sincronizado = true;
                                await db.UpdateAsync(tempm);*/
                            }
                            catch(Exception ex) {
                                await Reportarproceso("Error en actualizacion de dependencias de producto " + ex.Message + ex.StackTrace,
                                    true, JsonConvert.SerializeObject(i), "Carga de Producto");

                            }

                        }
                        catch (Exception ex)
                        {
                            await Reportarproceso("Error en actualizacion de dependencias de producto " + ex.Message + ex.StackTrace,
                                true, JsonConvert.SerializeObject(i), "Carga de Producto");

                        }
                    }
                    else {

                        await Reportarproceso("Error en carga de producto " + resultado.Errores,
                            true, JsonConvert.SerializeObject(i), "Carga de Producto");

                    }
                }
            }
            else
            {

            }
        }

        public async Task<resultfromLocalDB<Producto>> Listadeproductos()
        {
            try
            {
                //  resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
                var m = await db.Table<Producto>().ToListAsync();

                if (m.Count > 0)
                {
                    resultado.realizado = true;

                }
                else
                {
                    resultado.realizado = false;
                    resultado.Errores = "No se encontraron Productos";
                }
                return resultado;
            }
            catch {
                resultado.realizado = false;
                resultado.Errores = "No se encontraron Productos";
                return resultado;
            }
        }


        public override async Task clearData()
        {
            try
            {
                await db.ExecuteAsync("Delete from Producto where Sincronizado=1");
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<bool> verificarminimodefotos(Producto parametro)
        {
            var cantfotos = await db.QueryAsync<ProductoImagen>("select * from ProductoImagen where ProductoID is null and ProductoLocalID is null");
                //db.Table<ProductoImagen>()
               // .Where(x => (x.ProductoID == null && x.ProductoLocalID == null)) 
                //.ToListAsync();
            if (cantfotos == null)
                return false;
             if (cantfotos.Count >=3 && cantfotos.Count <=9)
            {
                return true;
            }
            else
            {
                await mensajetoast("Solo puede cargar un minimo de 3 y un maximo de 9 fotos por producto");
                return false;
            }
        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de productos: ");
            if (prueba)
                return;
            try
            {
                // SincronizacionData = true;
                await Reportarproceso("Descargando Productos ");

                resultfromAPIProducto datos = await repoapi.DescargaCatalogosProductos();
                if (datos.realizado)
                {
                    if (datos.Productos.Count>0) {
                        //await clearData();
                       await db.ExecuteAsync("Delete from Producto where (Sincronizado = 1 and ProductoID=0) or (Sincronizado = 1 and ProductoID is null)");

                        foreach (var producto in datos.Productos) {
                            var productopreexiste =await db.Table<Producto>().FirstOrDefaultAsync(x=>x.ProductoID==producto.ProductoID && x.ProductoID!=null && x.ProductoID!=0);
                            if (productopreexiste == null)
                            {
                                await base.insertarregistro(producto);
                            }
                        }
                        // await insertdata(datos.Productos, this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Productos " + datos.Errores,
                        true, "", "Descarga de productos");
                }
                //  SincronizacionData = false;
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Productos " + ex.Message + ex.StackTrace,
                    true, "", "Descarga de productos");

                // await mensajetoast(ex.StackTrace);
            }

        }

        public override async Task<resultfromLocalDB<List<Producto>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<Producto>> resultado = new resultfromLocalDB<List<Producto>>() { realizado = true, Errores = "" };

            try
            {
                var m = await db.Table<Producto>().ToListAsync();

                if (m.Count > 0)
                {
                    resultado.realizado = true;
                    resultado.Result = m;

                }
                else
                {
                    resultado.realizado = false;
                    resultado.Errores = "No se encontraron Productos";
                }
                return resultado;
            }
            catch
            {
                resultado.realizado = false;
                resultado.Errores = "No se encontraron Productos";
                return resultado;
            }
        }
    }

    }
