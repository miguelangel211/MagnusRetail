using System;
using System.Collections.Generic;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;

namespace CheckstoresMagnusRetail.ApiRepo
{

    public class genericresult {
        public bool realizado { get; set; }
        public string Errores { get; set; }
    }
    public  class resultfromAPI<T> :genericresult{
        public T Result { get; set; }

    }


    public class resultfromAPIUsuario:genericresult
    {
        public List<Usuario> Usuarios { get; set; }

    }
    public class resultfromAPIUsuarioServicio : genericresult
    {
        public List<ServicioUsuario> ServicioUsuarios { get; set; }

    }
    public class resultfromAPIProducto : genericresult
    {
        public List<Producto> Productos { get; set; }

    }
    public class resultfromAPIProgramas : genericresult {
        public List<Servicio> Programa { get; set; }
    }
    public class resultfromAPIStatusServicio:genericresult {
        public List<ServicioEstatus> ServicioEstatus { get; set; }
    }

    public class resultfromAPIMueble : genericresult {
        public List<ServicioMueble> Mueble { get; set; }
    }

    public class resultfromAPIMuebleTipo : genericresult
    {
        public List<MuebleTipo> MuebleTipo { get; set; }
    }

    public class resultfromAPIFotoMueble : genericresult {
        public List<ServicioMuebleImagen> MUebleImagen { get; set; }
    }

    public class resultfromAPIFotoLayout : genericresult {
        public List<ServicioLayout> LayoutImagen { get; set; }
    }

    public class resultfromAPIFotoLayoutBASE64   : genericresult
    {
        public List<ServicioLayoutapi> LayoutImagen { get; set; }
    }
    public class resultfromAPITramos : genericresult
    {
        public List<ServicioMuebleTramo> Tramo { get; set; }
    }
    public class resultfromAPICategorias : genericresult {
        public List<Categoria> Categorias { get; set; }
    }

    public class resultfromAPITramosNivel : genericresult
    {
        public List<ServicioMuebleTramoNivel>   TramoNivel { get; set; }
    }

    public class resultfromAPIMuebleTramosNivelCategoria : genericresult
    {
        public List<ServicioMuebleTramoNivelCategoria> MuebleTramoNivelCategoria { get; set; }
    }

    public class resultfromAPIFotosProducto: genericresult
    {
        public List<ProductoImagen> Fotos { get; set; }
    }


    public class resultfromAPIFotoscargaProducto : genericresult
    {
        public List<ProductoImagen> LayoutFotosSync { get; set; }
    }

    public class resultfromapiFotoMueble : genericresult {
        public List<ServicioMuebleImagen> MueblesFotosSync { get; set; }
    }

    public class resultfromAPIServicioMuebleProductoNivel : genericresult {
        public List<ServicioMuebleProductoNivel> ProductoNivel { get; set; }
    }


    public class resultfromAPIMueblePOST : genericresult
    {
        public List<ServicioMueble> MueblesSync { get; set; }
    }

    public class resultfromconcluirServicioPOST : genericresult
    {
        public List<Servicio> ConcluirServicioSync { get; set; }
    }

    public class resultfromAPITramoPOST : genericresult
    {
        public List<ServicioMuebleTramo> TramosSync { get; set; }
    }

    public class resultfromAPITramoNivelPOST : genericresult
    {
        public List<ServicioMuebleTramoNivel> TramoNivelsSync { get; set; }
    }

    public class resultfromAPIMuebleTramonivelCategoriaPOST : genericresult { 
  
        public List<ServicioMuebleTramoNivelCategoria> TramoNivelCategoriasSync { get; set; }
    }

    public class resultfromAPILayoutPOST : genericresult
    {

        public List<ServicioLayout> LayoutFotosSync { get; set; }
    }
    public class resultfromAPIProductoPOST : genericresult
    {

        public List<Producto> ProductoNuevosSync { get; set; }
    }
    public class resultfromAPIProductNivel : genericresult {
        public List<ServicioMuebleProductoNivel> ProductoNuevosSync { get; set; }
    }
    public class resultfromAPIProductoImagenPOST : genericresult
    {

        public List<ServicioLayout> LayoutFotosSync { get; set; }
    }
    public class Loginresult:Usuario {
        public string Error { get; set; }
        public bool Autenticado { get; set; }
    }
}
