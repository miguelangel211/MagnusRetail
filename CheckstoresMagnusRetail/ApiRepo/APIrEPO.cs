using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace CheckstoresMagnusRetail.ApiRepo
{
    public class ApiRequest:TableOperationsBase
    {
        HttpClient client;
        UriBuilder builder;
        string url = "";
        string urlerror ="";
        HttpResponseMessage response = null;
        int usuarioID;

        public ApiRequest()
        {
            client = new HttpClient();
            //   this.url = "https://checkstoresmagnusapi.azurewebsites.net/";
            // this.url = "http://bimagnus.eastus.cloudapp.azure.com/magnusretail/";
            this.url = " http://powerbinew.eastus.cloudapp.azure.com/magnusapi/";
            //  urlerror = "http://bimagnus.eastus.cloudapp.azure.com/magnuslog/api/dataerror/";
            urlerror = "http://powerbinew.eastus.cloudapp.azure.com/magnuslogging/";
           // urlerror = "http://datserver.ddns.net:8089/Log4net/api/dataerror/";
            getuser();
        }
        public async Task getuser()
        {
            var datos = await SecureStorage.GetAsync("User");
            var usuari = JsonConvert.DeserializeObject<Usuario>(datos);
            usuarioID = usuari.UsuarioID;
        }

        public async Task<genericresult> Pruebadeconexion()
        {
            string Errores = null;
            try
            {
                builder = new UriBuilder(string.Concat("http://powerbinew.eastus.cloudapp.azure.com/magnusapi"));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                if (response.IsSuccessStatusCode)
                {
                    return await Task.FromResult(new genericresult { realizado = true });
                }
                else {
                    return await Task.FromResult(new genericresult { realizado = false,Errores="No se pudo establecer una conexion" });

                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPIUsuario() { Usuarios = null };
                r.realizado = false;
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = "No se pudo establecer una conexion";
                }
                return await Task.FromResult(r);
            }
        }

        #region GETS
        public async Task<resultfromAPI<Loginresult>> LoginUsuario(string usuario, string password)
        {
            string Errores = null;

            try
            {
                builder = new UriBuilder(string.Concat(url,"API/account/login"));
                builder.Port = -1;

                var query = HttpUtility.ParseQueryString(builder.Query);
                query["UserName"] = usuario;
                query["Password"] = password;
                query["UsuarioID"] = usuarioID.ToString();

                builder.Query = query.ToString();
                string urlbuild = builder.ToString();


                response = await client.PostAsync(urlbuild, null);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {

                    var reso = JsonConvert.DeserializeObject<resultfromAPI<Loginresult>>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPI<Loginresult>
                    {
                        realizado = respuestageneric.realizado,
                        Result = new Loginresult(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch(Exception ex) 
            {
                //UserDialogs.Instance.Toast(ex.Message);

                var r = new resultfromAPI<Loginresult>() { Result = new Loginresult { Autenticado = false } };
                r.realizado = false;
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIUsuario> DescargaCatalogosUsuarios()
        {
            string Errores= null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/Usuarios/Descarga?UsuarioID="+usuarioID));
                builder.Port = -1;

                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();

                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIUsuario>(r);
                    return await Task.FromResult(reso);
                }
                else {
                    return await Task.FromResult(new resultfromAPIUsuario { realizado = respuestageneric.realizado,
                        Usuarios = new List<Usuario>(),
                        Errores = respuestageneric.Errores }); ;
                }
        
            }
            catch (Exception ex)
            {
                var r = new resultfromAPIUsuario() { Usuarios=null};
                r.realizado = false;
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message; //+ " : " + ex.StackTrace;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIProducto> DescargaCatalogosProductos()
        {
            string Errores=null;
            try
            {

               string usuariodata =await SecureStorage.GetAsync("User");
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuariodata);
                builder = new UriBuilder(string.Concat(url, "API/Productos/Descarga?UsuarioID="+usuario.UsuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();

                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {

                    var reso = JsonConvert.DeserializeObject<resultfromAPIProducto>(r);
                    foreach (var p in reso.Productos)
                    {
                        p.Sincronizado = true;
                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIProducto
                    {
                        realizado = respuestageneric.realizado,
                        Productos = new List<Producto>(),
                        Errores = respuestageneric.Errores
                    }); 
                }


            }
            catch (Exception ex)
            {
                var r = new resultfromAPIProducto() { Productos = null };
                r.realizado = false;
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message;// + " : " + ex.StackTrace;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIProgramas> DescargaprogramasUsuario()
        {
            string Errores = null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/Programas/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIProgramas>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIProgramas
                    {
                        realizado = respuestageneric.realizado,
                        Programa = new List<Servicio>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                var r = new resultfromAPIProgramas() { Programa = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }
        public async Task<resultfromAPIStatusServicio> Descargaestatusservicio()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/ServicioEstatus/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIStatusServicio>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIStatusServicio
                    {
                        realizado = respuestageneric.realizado,
                        ServicioEstatus = new List<ServicioEstatus>(),
                        Errores = respuestageneric.Errores
                    });
                }


            }
            catch (Exception ex)
            {
                var r = new resultfromAPIStatusServicio() { ServicioEstatus = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIMuebleTipo> DescargaTiposMuebles()
        {
            string Errores = null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/MuebleTipo/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIMuebleTipo>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIMuebleTipo
                    {
                        realizado = respuestageneric.realizado,
                        MuebleTipo = new List<MuebleTipo>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPIMuebleTipo() { MuebleTipo = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }


        public async Task<resultfromAPIMueble> DescargaMuebles()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/Mueble/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIMueble>(r);
                    foreach (var m in reso.Mueble)
                    {
                        m.Sincronizado = true;
                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIMueble
                    {
                        realizado = respuestageneric.realizado,
                        Mueble = new List<ServicioMueble>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPIMueble() { Mueble = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIFotoMueble> DescargaMueblesServicioImagen()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/MuebleImagen/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();

                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIFotoMueble>(r);
                    foreach (var item in reso.MUebleImagen)
                    {
                        var webClient = new WebClient();
                        try
                        {
                            var re = await webClient.DownloadDataTaskAsync(item.URLFoto);
                            item.MuebleImagen = re;
                        }
                        catch(Exception ex) {
                            await Reportarproceso("Error al descargar imagen de muebles " + ex.Message, false, JsonConvert.SerializeObject(item), "Descarga de imagen de mueble");

                        }
                        item.Sincronizado = true;


                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIFotoMueble
                    {
                        realizado = respuestageneric.realizado,
                        MUebleImagen = new List<ServicioMuebleImagen>(),
                        Errores = respuestageneric.Errores
                    });
                }



            }
            catch (Exception ex)
            { 
                var r = new resultfromAPIFotoMueble() { MUebleImagen = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIFotoLayout> DescargaLayoutImagen()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/LayoutImagen/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIFotoLayout>(r);
                    foreach (var item in reso.LayoutImagen)
                    {
                        var webClient = new WebClient();
                        try
                        {
                            item.LayoutImagen = await webClient.DownloadDataTaskAsync(item.URLFoto);
                        }
                        catch(Exception ex) {
                            await Reportarproceso("Error al descargar layout " + ex.Message, false, JsonConvert.SerializeObject(item), "Descarga de imagen de layout");

                        }
                        item.Sincronizado = true;
                    }

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIFotoLayout
                    {
                        realizado = respuestageneric.realizado,
                        LayoutImagen = new List<ServicioLayout>(),
                        Errores = respuestageneric.Errores
                    });
                }


            }
            catch (Exception ex)
            {
                var r = new resultfromAPIFotoLayout() { LayoutImagen = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIFotosProducto> DescargaProductoImagenes(string prodid)
        {
            string Errores=null;
            try
            {
                string usuariodata =await SecureStorage.GetAsync("User");
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuariodata);
                builder = new UriBuilder(string.Concat(url, "API/Productos/DescargaFotos?idSiguiente="+prodid+ "&UsuarioID="+usuario.UsuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();

                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {

                    var reso = JsonConvert.DeserializeObject<resultfromAPIFotosProducto>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIFotosProducto
                    {
                        realizado = respuestageneric.realizado,
                        Fotos = new List<ProductoImagen>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPIFotosProducto() { Fotos = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPICategorias> DescargaCategorias()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/Categoria/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPICategorias>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPICategorias
                    {
                        realizado = respuestageneric.realizado,
                        Categorias = new List<Categoria>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPICategorias() { Categorias = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPITramos> DescargaTramos()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/Tramos/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPITramos>(r);
                    foreach (var m in reso.Tramo)
                    {
                        m.Sincronizado = true;
                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPITramos
                    {
                        realizado = respuestageneric.realizado,
                        Tramo = new List<ServicioMuebleTramo>(),
                        Errores = respuestageneric.Errores
                    });
                }



            }
            catch (Exception ex)
            {
                var r = new resultfromAPITramos() { Tramo = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPITramosNivel> DescargaTramosNivel()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/TramosNivel/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPITramosNivel>(r);
                    foreach (var m in reso.TramoNivel)
                    {
                        m.Sincronizado = true;
                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPITramosNivel
                    {
                        realizado = respuestageneric.realizado,
                        TramoNivel = new List<ServicioMuebleTramoNivel>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                var r = new resultfromAPITramosNivel() { TramoNivel = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIUsuarioServicio> DescargaUsuariosServicio()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/ServicioUsuarios/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIUsuarioServicio>(r);
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIUsuarioServicio
                    {
                        realizado = respuestageneric.realizado,
                        ServicioUsuarios = new List<ServicioUsuario>(),
                        Errores = respuestageneric.Errores
                    });
                }


            }
            catch (Exception ex)
            {
                var r = new resultfromAPIUsuarioServicio() { ServicioUsuarios = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }


        public async Task<resultfromAPIMuebleTramosNivelCategoria> DescargaMuebleTramoNivelCategoria ()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/MuebleTramoNivelCategoria/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIMuebleTramosNivelCategoria>(r);
                    foreach (var m in reso.MuebleTramoNivelCategoria)
                    {
                        m.Sincronizado = true;
                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIMuebleTramosNivelCategoria
                    {
                        realizado = respuestageneric.realizado,
                        MuebleTramoNivelCategoria = new List<ServicioMuebleTramoNivelCategoria>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                var r = new resultfromAPIMuebleTramosNivelCategoria() { MuebleTramoNivelCategoria = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }



        public async Task<resultfromAPIServicioMuebleProductoNivel> DescargaServicioMuebleProductoNivel()
        {
            string Errores=null;
            try
            {
                builder = new UriBuilder(string.Concat(url, "API/ProductoNivel/Descarga?UsuarioID=" + usuarioID));
                builder.Port = -1;
                string urlbuild = builder.ToString();
                response = await client.GetAsync(urlbuild);
                string r = await response.Content.ReadAsStringAsync();
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIServicioMuebleProductoNivel>(r);
                    foreach (var re in reso.ProductoNivel)
                    {
                        re.Sincronizado = true;
                    }
                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIServicioMuebleProductoNivel
                    {
                        realizado = respuestageneric.realizado,
                        ProductoNivel = new List<ServicioMuebleProductoNivel>(),
                        Errores = respuestageneric.Errores
                    });
                }


            }
            catch (Exception ex)
            {
                var r = new resultfromAPIServicioMuebleProductoNivel() { ProductoNivel = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }
        #endregion GETS

        #region POSTS
        public async Task<resultfromAPIMueblePOST> CargarMuebles(ServicioMueble parametro)
        {
            string respuesta = "";
            string Errores = null;
            try
            {

                if (parametro.MuebleComentario == null) {
                    parametro.MuebleComentario = " ";
                }
                var dato = new List<ServicioMueble>();
                dato.Add(parametro);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/Mueble/Carga"), content);

                respuesta = await response.Content.ReadAsStringAsync();
                
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(respuesta);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    return await Task.FromResult(JsonConvert.DeserializeObject<resultfromAPIMueblePOST>(respuesta));

                }
                else
                {
                    return await Task.FromResult(new resultfromAPIMueblePOST
                    {
                        realizado = respuestageneric.realizado,
                        MueblesSync = new List<ServicioMueble>(),
                        Errores = respuestageneric.Errores
                    }) ;
                }
            }
            catch (Exception ex)
            {
                //   UserDialogs.Instance.Toast(ex.Message);

                var r = new resultfromAPIMueblePOST();
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPITramoPOST> CargarTramosPOST(ServicioMuebleTramo parametro)
        {
            string respuesta="";
            string Errores=null;
            try
            {

                var dato = new List<ServicioMuebleTramo>();
                dato.Add(parametro);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/Tramos/Carga"), content);
                respuesta = await response.Content.ReadAsStringAsync();

                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(respuesta);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    return await Task.FromResult(JsonConvert.DeserializeObject<resultfromAPITramoPOST>(respuesta));
                }
                else
                {
                    return await Task.FromResult(new resultfromAPITramoPOST
                    {
                        realizado = respuestageneric.realizado,
                        TramosSync = new List<ServicioMuebleTramo>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                //   UserDialogs.Instance.Toast(ex.Message);

                var r = new resultfromAPITramoPOST();
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPITramoNivelPOST> CargarTramosNivelPOST(ServicioMuebleTramoNivel parametro)
        {
            string respuesta = "";
            string Errores = null;
            try
            {
                var dato = new List<ServicioMuebleTramoNivel>();
                dato.Add(parametro);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/TramosNivel/Carga"), content);
                respuesta = await response.Content.ReadAsStringAsync();

                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(respuesta);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    return await Task.FromResult(JsonConvert.DeserializeObject<resultfromAPITramoNivelPOST>(respuesta));
                }
                else
                {
                    return await Task.FromResult(new resultfromAPITramoNivelPOST
                    {
                        realizado = respuestageneric.realizado,
                        TramoNivelsSync = new List<ServicioMuebleTramoNivel>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                //   UserDialogs.Instance.Toast(ex.Message);

                var r = new resultfromAPITramoNivelPOST();
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIMuebleTramonivelCategoriaPOST> CargaMuebleTramoNivelCategoriaPOST(ServicioMuebleTramoNivelCategoria parameter)
        {
            string Errores = null;
            string respuesta = "";
            try
            {

                var dato = new List<ServicioMuebleTramoNivelCategoria>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/MuebleTramoNivelCategoria/Carga"), content);
                respuesta = await response.Content.ReadAsStringAsync();
               
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(respuesta);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    return await Task.FromResult(JsonConvert.DeserializeObject<resultfromAPIMuebleTramonivelCategoriaPOST>(respuesta));
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIMuebleTramonivelCategoriaPOST
                    {
                        realizado = respuestageneric.realizado,
                        TramoNivelCategoriasSync = new List<ServicioMuebleTramoNivelCategoria>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                var r = new resultfromAPIMuebleTramonivelCategoriaPOST() { TramoNivelCategoriasSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPILayoutPOST> CargaLayoutPOST(ServicioLayout parameter)
        {
            string Errores = null;
            string respuesta="";
            try
            {

                var dato = new List<ServicioLayout>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/LayoutImagen/CargaFotos"), content);
                string r = await response.Content.ReadAsStringAsync();
                respuesta = r;
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPILayoutPOST>(r);

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPILayoutPOST
                    {
                        realizado = respuestageneric.realizado,
                        LayoutFotosSync = new List<ServicioLayout>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPILayoutPOST() { LayoutFotosSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIProductoPOST> CargaProductoPOST(Producto parameter)
        {
            string Errores=null;
            string respuesta = "";
            try
            {
                var dato = new List<Producto>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/Productos/Carga"), content);
                string r = await response.Content.ReadAsStringAsync();
                respuesta = r;
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIProductoPOST>(r);

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIProductoPOST
                    {
                        realizado = respuestageneric.realizado,
                        ProductoNuevosSync = new List<Producto>(),
                        Errores = respuestageneric.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                var r = new resultfromAPIProductoPOST() { ProductoNuevosSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromAPIProductNivel> CargaProductoNivelPOST(ServicioMuebleProductoNivel parameter)
        {
            string Errores = null;
            string respuesta = "";
            try
            {
                var dato = new List<ServicioMuebleProductoNivel>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/ProductoNivel/Carga"), content);
                string r = await response.Content.ReadAsStringAsync();
                respuesta = r;
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIProductNivel>(r);

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIProductNivel
                    {
                        realizado = respuestageneric.realizado,
                        ProductoNuevosSync = new List<ServicioMuebleProductoNivel>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPIProductNivel() { ProductoNuevosSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }


        public async Task<resultfromAPIFotoscargaProducto> CargaProductoImagenPOST(ProductoImagen parameter)
        {
            string Errores=null;
            string respuesta = "";
            try
            {

                var dato = new List<ProductoImagen>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/Productos/CargaFotos"), content);
                string r = await response.Content.ReadAsStringAsync();
                respuesta = r;
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromAPIFotoscargaProducto>(r);

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromAPIFotoscargaProducto
                    {
                        realizado = respuestageneric.realizado,
                        LayoutFotosSync = new List<ProductoImagen>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromAPIFotoscargaProducto() { LayoutFotosSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message// + " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromapiFotoMueble> CargaMuebleImagenPOST(ServicioMuebleImagen parameter)
        {
            string Errores=null;
            string respues = "";
            try
            {

                var dato = new List<ServicioMuebleImagen>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/MuebleImagen/CargaFotos"), content);
                string r = await response.Content.ReadAsStringAsync();
                respues = r;
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromapiFotoMueble>(r);

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromapiFotoMueble
                    {
                        realizado = respuestageneric.realizado,
                        MueblesFotosSync = new List<ServicioMuebleImagen>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromapiFotoMueble() { MueblesFotosSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<resultfromconcluirServicioPOST> ConcluirServicio(Servicio parameter)
        {
            string respuesta = "";
            string Errores = null;
            try
            {

                var dato = new List<Servicio>();
                dato.Add(parameter);
                HttpResponseMessage response = null;
                string json = JsonConvert.SerializeObject(dato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(string.Concat(url, "api/Programas/ConcluirServicio"), content);
                string r = await response.Content.ReadAsStringAsync();
                respuesta = r;
                var respuestageneric = JsonConvert.DeserializeObject<genericresult>(r);
                Errores = respuestageneric.Errores;
                if (respuestageneric.realizado)
                {
                    var reso = JsonConvert.DeserializeObject<resultfromconcluirServicioPOST>(r);

                    return await Task.FromResult(reso);
                }
                else
                {
                    return await Task.FromResult(new resultfromconcluirServicioPOST
                    {
                        realizado = respuestageneric.realizado,
                        ConcluirServicioSync = new List<Servicio>(),
                        Errores = respuestageneric.Errores
                    });
                }

            }
            catch (Exception ex)
            {
                var r = new resultfromconcluirServicioPOST() { ConcluirServicioSync = null };
                r.realizado = false;
                Debug.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(Errores))
                {
                    r.Errores = Errores;
                }
                else
                {
                    r.Errores = ex.Message //+ " : " + ex.StackTrace
                        ;
                }
                return await Task.FromResult(r);
            }
        }

        public async Task<genericresult> postapiAeeorAsync(string error)
        {

            try
            {
                HttpResponseMessage response = null;

                string json = error;
                var content = new StringContent(json, Encoding.UTF8, "application/json");
               await client.PostAsync(string.Concat(urlerror, "Registrarerror"), content);
               // string r = await response.Content.ReadAsStringAsync();
                return await Task.FromResult(new genericresult {realizado=true,Errores="" });
            }
            catch (Exception ex)
            {

                return await Task.FromResult(new genericresult {realizado=false,Errores=ex.Message });
            }
        }
        #endregion POSTS


    }
}
