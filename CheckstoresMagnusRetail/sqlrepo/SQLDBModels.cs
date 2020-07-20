using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.sqlrepo
{
    public class resultfromLocalDB<T>
    {
        public T Result { get; set; }
        public bool realizado { get; set; }
        public string Errores { get; set; }
    }


    public abstract class BaseDBmodel:INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Usuario{
        [PrimaryKey]
        public int UsuarioID { get; set; }
        [Column("RoleID")]
        public int RolID { get; set; }
        public string Nombre { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        //[JsonIgnore]
        public byte[] Foto { get; set; }
        public string FotoRuta { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaHoraCreacion { get; set; }
        public DateTime FechaHoraActualiza { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        //[JsonProperty("Foto"),Ignore]
        //public string imagendata { get; set; }
    }

    public class ServicioEstatus {
        [PrimaryKey]
        public int ServicioEstatusID { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHoraCreacion { get; set; }
        public DateTime? FechaHoraModifica { get; set; }
    }

    public class Servicio {
        [PrimaryKey]
        public int ServicioID { get; set; }
        [Column("ServicioTipo")]
        public string TipoServicio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int? ServicioEstatusID { get; set; }
        public DateTime? ServicioFechaHora { get; set; }
        public string NombreCadena { get; set; }
        public string NombreTienda { get; set; }
        public string Ubicacion { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; } = true;
        [Ignore, JsonIgnore]
        public int UsuarioID { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        [Ignore]
        public virtual ServicioEstatus ServicioEstatus {get;set;}
    }


    public class ServicioUsuario {
        [PrimaryKey]
        public int ServicioID { get; set; }
        public int UsuarioID { get; set; }

    }

    public class ServicioLayout {
        [PrimaryKey]
        public int ServicioID { get; set; }
        [PrimaryKey,AutoIncrement]
        public int? ServicioLayoutLocalID { get; set; }
        public int ServicioLayoutID { get; set; }
        public byte[] LayoutImagen { get; set; }
        public string LayoutRuta { get; set; }
        [Ignore]
        public string URLFoto { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        [Ignore]
        public string ErrorCarga { get; set; }

    }



    public class Producto:BaseDBmodel {
        [PrimaryKey, AutoIncrement,NotNull]
        public int? ProductoLocalID { get; set; }
        public int? ProductoID { get; set; }
        public string upc;
        public string UPC { get { return upc; } set { upc = value;OnPropertyChanged(); } }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Fabricante { get; set; }
        public string Alto { get; set; }
        public string Ancho { get; set; }
        public string Gramaje { get; set; }
        public string Profundo { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        public int? categoriaid;
        public int? CategoriaID { get { return categoriaid; } set { categoriaid = value; OnPropertyChanged(); }  }

        public Categoria categoria;
        [Ignore,JsonIgnore]
        public Categoria Categoria { get { return categoria; } set { categoria = value;OnPropertyChanged(); } }
    }


    public class ProductoImagen {
        [PrimaryKey,AutoIncrement]
        public int? ProductoImagenLocalID { get; set; }
        public int? ProductoImagenID { get; set; }
        public int? ProductoID { get; set; }
        [Column("ProductoImagen"),JsonProperty("ProductoImagen")]
        public byte[] ProductoImagen1 { get; set; }
        public string ProductImageRuta { get; set; }
        public int? ProductoLocalID { get; set; }
        public string DispositivoID { get; set; }
        [Ignore]
        public string URLFoto { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
    }



    public class MuebleTipo {
        [PrimaryKey]
        public int MuebleTipoID { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaHoraCreacion { get; set; }
        public DateTime? FechahoraModifica { get; set; }
    }

    public class ServicioMueble:BaseDBmodel
    {
        [SQLite.PrimaryKey]
        public int ServicioID { get; set; }
        [SQLite.PrimaryKey, SQLite.AutoIncrement, NotNull]
        public int? ServicioMuebleLocalID { get; set; }
        public int MuebleTipoID { get; set; }
        public int? ServicioMuebleID { get; set; }
        //[Column("MueblaCara")]
       
        public string MuebleCara { get; set; }
        public string MueblePasillo { get; set; }
        public string MuebleTramoInicial { get; set; }
        public string MuebleComentario { get; set; }
       
        public decimal? MuebleAltura { get; set; }
        public int? MuebleTramos { get; set; }
        public bool MuebleMedidasIguales { get; set; }
        public int? MuebleNivelMinimo { get; set; }
        public int? MuebleNivelMaximo { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }


        #region notificacionesdeeeror
        public bool caraError;
        [Ignore]
        public bool CaraError { get { return caraError; } set { caraError = value;OnPropertyChanged(); } }
        public bool pasilloerror;
        [Ignore]
        public bool PasilloError { get { return pasilloerror; } set { pasilloerror = value;OnPropertyChanged(); } }
        #endregion
    }



    public class ServicioMuebleTramo: BaseDBmodel
    {
            [PrimaryKey]
            public int ServicioID { get; set; }
            [PrimaryKey,AutoIncrement,NotNull]
            public int ServicioMuebleTramoLocalID { get; set; }
            public int? ServicioMuebleLocalID { get; set; }
            public int? ServicioMuebleTramoID { get; set; }
            public int? ServicioMuebleID { get; set; }
            public int? TramoNumero { get; set; }
            public string DispositivoID { get; set; }
            public string DispositivoNombre { get; set; }
            public DateTime? FechaHoraLocal { get; set; }
            public int UsuarioLocalID { get; set; }
            public bool Sincronizado { get; set; }
            public DateTime? SincronizadoFecha { get; set; }
            [Ignore]
            public string TramoName { get; set; }
            public ObservableCollection<ServicioMuebleTramoNivel> niveles;
            [Ignore]
            public ObservableCollection<ServicioMuebleTramoNivel> Niveles { get { return niveles; } set { niveles = value; OnPropertyChanged(); } }
            [Ignore]
            public Categoria Categoria { get; set; }

    }

    public class ServicioMuebleTramoNivel:BaseDBmodel
    {
        [PrimaryKey]
        public int ServicioMuebleTramoNivelID { get; set; }
        [PrimaryKey,AutoIncrement,NotNull]
        public int? ServicioMuebleTramoNivelLocalID { get; set; }
        public int ServicioID { get; set; }
        public int ServicioMuebleTramoLocalID { get; set; }
        public int ServicioMuebleLocalID { get; set; }
        public int? ServicioMuebleTramoID { get; set; }
        public int? ServicioMuebleID { get; set; }
        public int? profundo;
        public int? Profundo { get { return profundo; } set { profundo = value; OnPropertyChanged(); } }
        public int?  ancho { get; set; }
        public int? Ancho { get { return ancho; } set { ancho = value;OnPropertyChanged(); } }
        public int? alto { get; set; }
        public int? Alto { get { return alto; } set { alto = value;OnPropertyChanged(); } }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        [Ignore]
        public string NombreNivel{ get; set; }
        [Ignore]
        public bool ActivaEdicion { get; set; }
        [Ignore]
        public int NumeroNivel { get; set; }
        [Ignore,JsonIgnore]
        public bool Editable { get; set; }
    }

    public class ServicioMuebleProductoNivel {
        [PrimaryKey,AutoIncrement,NotNull]
        public int? ServicioMuebleProductoNivelLocalID { get; set; }
        [PrimaryKey]
        public int ServicioMuebleTramoNivelLocalID { get; set; }
        public int? ServicioMuebleTramoNivelID { get; set; }
        public int? ServicioMuebleProductoNivelID { get; set; }
        public int? Frente { get; set; }
        public int? Profundo { get; set; }
        [Column("Posicion")]
        public int? Posicion { get; set; }
        [Ignore]
        public int ServicioID { get; set; }

        public int ProductoLocalID { get; set; }
        public int? ProductoID { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        [Ignore]
        public ServicioMuebleTramoNivel Nivel { get; set; }
        [Ignore]
        public ServicioMuebleTramo tramo { get; set; }
        [Ignore]
        public Producto producto { get; set; }
        [Ignore]
        public string NombreNivel { get; set; }
        public int ServicioMuebleTramoNivelCategoriaLocalID { get; set; }
        public int? ServicioMuebleTramoNivelCategoriaID { get; set; }
        public int CategoriaLocalID { get; set; }
        public int CategoriaID { get; set; }
        [Ignore]
        public ServicioMuebleTramoNivelCategoria nivelcategoria { get; set; }
        [Ignore]
        public bool Editable { get; set; } = false;

    }

    public class ServicioMuebleImagen {
        [PrimaryKey]
        public int ServicioID { get; set; }
        [PrimaryKey,AutoIncrement,NotNull]
        public int? ServicioMuebleImagenLocalID { get; set; }
        public int ServicioMuebleLocalID { get; set; }
        public int? ServicioMuebleImagenID { get; set; }
        public int? ServicioMuebleID { get; set; }
        public byte[] MuebleImagen { get; set; }
        public string MuebleImagenRuta { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        [Ignore]
        public string URLFoto { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
    }

    public class Categoria {
        [PrimaryKey,AutoIncrement,NotNull]
        public int? CategoriaLocalID { get; set; }
        public int? CategoriaID { get; set; }
        public string CategoriaNombre { get; set; }
        public byte?[] CategoriaImagen { get; set; }
        public string CategoriaImagenRuta { get; set; }
        public bool Activo { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        [Ignore]
        public bool Editable { get; set; } = true;
    }

    public class ServicioMuebleTramoNivelCategoria {
        [PrimaryKey]
        public int? CategoriaLocalID { get; set; }
        [PrimaryKey,AutoIncrement,NotNull]
        public int? ServicioMuebleTramoNivelCategoriaLocalID { get; set; }
        public int? ServicioMuebleTramoNivelCategoriaID { get; set; }
        public int? ServicioMuebleTramoID { get; set; }
        public int? ServicioMuebleTramoLocalID { get; set; }
        public int? ServicioMuebleTramoNivelLocalID { get; set; }
        public int? ServicioMuebleTramoNivelID { get; set; }
        public int? CategoriaID { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        [Ignore]
        public int ServicioID { get; set; }
    }

    public class ServicioMuebleCategoria {
        [PrimaryKey]
        public int ServicioID { get; set; }
        [PrimaryKey,AutoIncrement,NotNull]
        public int? ServicioMuebleLocalID { get; set; }
        [PrimaryKey]
        public int CategoriaLocalID { get; set; }
        public int? CategoriaID { get; set; }
        public int? ServicioMuebleID { get; set; }
    }
}
