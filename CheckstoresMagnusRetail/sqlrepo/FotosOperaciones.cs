using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;

namespace CheckstoresMagnusRetail.sqlrepo
{
    public class FotoProductoOperaciones : ITableoperations<ProductoImagen>
    {

        public override async Task<resultfromLocalDB<ProductoImagen>> consultarDatoconcisoAsync(ProductoImagen parameter)
        {
            // resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
            try
            {
                resultado.Result = await db.Table<ProductoImagen>().FirstOrDefaultAsync(x => x.ProductoImagenID == parameter.ProductoImagenID);
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
        public override async Task insertarregistro(ProductoImagen parameter)
        {
            try
            {
                await Reportarproceso("Guardando Fotos del producto " + parameter.ProductoID);
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
            var prueba = await probarred("Carga de fotos de Producto: ");
            if (prueba)
                return;

            var m = await db.Table<ProductoImagen>().Where(x => (x.Sincronizado == false || x.Sincronizado == null)&&(x.ProductoID!=0 && x.ProductoID!=null)).ToListAsync();
            if (m.Count > 0)
            {

                foreach (var i in m)
                {
                   // i.DispositivoID = DispositivoID;
                   // i.DispositivoNombre = DispositivoName;
                    await Reportarproceso("Cargando Foto del Producto local "+i.ProductoImagenLocalID);
                    var r = await repoapi.CargaProductoImagenPOST(i);
                    if (r.realizado)
                    {
                        try
                        {var resulta= r.LayoutFotosSync.FirstOrDefault().ProductoImagenID;
                            await db.ExecuteAsync("update ProductoImagen set Sincronizado=1,ProductoImagenID=? where ProductoImagenLocalID=?", resulta,i.ProductoImagenLocalID);
                            /*
                            var tempm = await db.Table<ProductoImagen>().FirstOrDefaultAsync(x => x.ProductoImagenLocalID == i.ProductoImagenLocalID);
                            tempm.ProductoImagenID = r.LayoutFotosSync.FirstOrDefault().ProductoImagenID;
                            tempm.Sincronizado = true;
                            await db.UpdateAsync(tempm);*/
                        }
                        catch(Exception ex) {
                            await Reportarproceso("Error en actualizacion de Foto de Producto local sincronizado "
                                + ex.Message + ex.StackTrace, true, i.ProductoImagenLocalID, "Carga de fotos de producto");

                        }
                    }
                    else {
                        await Reportarproceso("Error en carga de Foto de Producto local "+ r.Errores, true, i.ProductoImagenLocalID, "Carga de fotos de producto");

                    }
                }
            }
            else
            {

            }
        }

        public async Task<resultfromLocalDB<List<ProductoImagen>>> ListadeimagenessAsync()
        {
            resultfromLocalDB<List<ProductoImagen>> resultado = new resultfromLocalDB<List<ProductoImagen>>() { realizado = true, Errores = "" };
            try
            {
                //  resultfromLocalDB<Usuario> resultado = new resultfromLocalDB<Usuario>() { realizado = true, Errores = "" };
                var m = await db.Table<ProductoImagen>().ToListAsync();

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


        public override async Task clearData()
        {
           // await db.ExecuteAsync("Delete from ProductoImagen where Sincronizado=1");
        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de fotos: ");
            if (prueba)
                return;
            List<Task> tasklist = new List<Task>();
            try
            {
                int operaciones = 0;
                // SincronizacionData = true;
              //  var last = productos.Last();
                await clearData();
                bool realizado=true;
                int callnumber = 0;
                var webClient = new WebClient();

                do
                {
                    await Reportarproceso("Descargando Fotos de producto desde:  " + callnumber.ToString());
                    resultfromAPIFotosProducto datos = await repoapi.DescargaProductoImagenes(callnumber.ToString());
                    if (datos.realizado)
                    {
                        // await clearData();

                        if (datos.Fotos.Count > 0)
                        {

                            foreach (var f in datos.Fotos)
                            {

                                var exist = await db.Table<ProductoImagen>().
                                    FirstOrDefaultAsync(x=>x.ProductoImagenID==f.ProductoImagenID && x.ProductoImagen1!=null);
                                    
                                if (exist==null){
                                    try
                                    {


                                            f.ProductoImagen1 = await webClient.DownloadDataTaskAsync(f.URLFoto);
                                        f.Sincronizado = true;
                                        await insertarregistro(f);
                                    }
                                    catch(Exception ex) {
                                        await Reportarproceso("Error al descargar imagen "+ex.Message,false,JsonConvert.SerializeObject(f),"Descarga de imagen de producto");
                                        Debug.WriteLine(ex.Message);
                                    }

                                }
                            }
                           // await Task.WhenAll(tasklist);
                            if (datos.Fotos.Last().ProductoID.HasValue)
                            {
                                callnumber = datos.Fotos.Last().ProductoID ?? 0;
                                callnumber++;
                            }
                            else {
                                realizado = false;
                            }
                        }
                    }
                    else
                    {
                        await Reportarproceso("Error al descargar Fotos de Producto " + datos.Errores, true, "", "Descarga de fotos  de producto");

                        realizado = false;
                    }
                } while (realizado) ;



            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Fotos de Producto " + ex.Message + ex.StackTrace, true, "", "Descarga de fotos de producto");

                Debug.WriteLine(ex.Message);

                //await mensajetoast(ex.StackTrace);
            }


        }

        public override async Task<resultfromLocalDB<List<ProductoImagen>>> consultarListadodedata(object parameter)
        {
            resultfromLocalDB<List<ProductoImagen>> resultado = new resultfromLocalDB<List<ProductoImagen>>() { realizado = true, Errores = "" };
            try
            {
                var parametro = (parameter as Producto);
                //  var m = await db.QueryAsync<ProductoImagen>("select * from ProductoImagen" +
                //    "where ProductoID=? and ");
                var s = await db.Table<ProductoImagen>().ToListAsync();
                var m= await  db.Table<ProductoImagen>().
                   Where(
                     x=>(x.ProductoID==parametro.ProductoID && parametro.ProductoID!=null) ||
                     (x.ProductoID==null && x.ProductoLocalID == parametro.ProductoLocalID &&
                     x.ProductoLocalID!=null && parametro.ProductoID==null)||
                     (x.ProductoLocalID==null && x.ProductoID==null && parametro.ProductoLocalID==null && parametro.ProductoID==null)
                     ).
                    ToListAsync();
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
            var imagenaborrar = await db.Table<ProductoImagen>().FirstOrDefaultAsync(x => x.ProductoImagenLocalID == ID);
            await db.DeleteAsync(imagenaborrar);
        }
    }
}
