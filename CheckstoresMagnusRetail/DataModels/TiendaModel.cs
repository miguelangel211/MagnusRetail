using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckstoresMagnusRetail.ViewModels;
using CheckstoresMagnusRetail.sqlrepo;
namespace CheckstoresMagnusRetail.DataModels
{
    public class TiendaModel:ObservableCollection<sqlrepo.Servicio>,INotifyPropertyChanged
    {
        public bool expanded;
       public bool Expanded { get { return expanded; }set { expanded = value; OnPropertyChanged("Expanded"); } }
        public TiendaDataModel Heading { get; set; }
        public ObservableCollection<sqlrepo.Servicio> Servicios => this;
        public List<sqlrepo.Servicio> Serviciosstorage { get; set; } = new List<Servicio>();
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void expandir()
        {
            foreach (var t in Serviciosstorage)
            {
                Servicios.Add(t);
            }
        }

    }
    public class ServicioLayoutapi
    {
        public int ServicioID { get; set; }
        public int? ServicioLayoutLocalID { get; set; }
        public int ServicioLayoutID { get; set; }
        public String LayoutImagen { get; set; }
        public string LayoutRuta { get; set; }
        public string DispositivoID { get; set; }
        public string DispositivoNombre { get; set; }
        public DateTime? FechaHoraLocal { get; set; }
        public int? UsuarioLocalID { get; set; }
        public bool? Sincronizado { get; set; }
        public DateTime? SincronizadoFecha { get; set; }
        public string ErrorCarga { get; set; }

    }

    public class TiendaDataModel: IEquatable<TiendaDataModel>
     {
        public string NombreCadena { get; set; }
        public string NombreTienda { get; set; }
        public string Ubicacion { get; set; }
        public bool Equals(TiendaDataModel other)
        {
            if (NombreCadena == other.NombreCadena && NombreTienda == other.NombreTienda && Ubicacion == other.Ubicacion)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashNombreCadena = NombreCadena == null ? 0 : NombreCadena.GetHashCode();
            int hashNombreTienda = NombreTienda == null ? 0 : NombreTienda.GetHashCode();
            int hashUbicacion = Ubicacion == null ? 0 : Ubicacion.GetHashCode();

            return hashNombreCadena ^ hashNombreTienda ^ hashUbicacion;
        }
    }



    public class MueblesModel:ObservableCollection<TramoModel>,INotifyPropertyChanged
    {
     
        public MuebleModel Mueble { get; set; }
        public string image;
        public string Image { get { return image; } set { image = value;OnPropertyChanged(); } }

        public bool exnoanded;
        public bool Expanded { get { return exnoanded; } set { exnoanded = value;if (value == true) {
                    Image = "hide.png";
                } else {
                    Image = "expand.png";
                } OnPropertyChanged(); } }
        public ObservableCollection<TramoModel> Tramos => this;
        public ObservableCollection<TramoModel> Tramosstorage { get; set; } = new ObservableCollection<TramoModel>();
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void expandir() {
            foreach (var t in Tramosstorage) {
                Tramos.Add(t);
            }
        }
    }

   

    public class Catgoria{
        public bool Activo { get; set; }
        public string Categoria { get; set; }
    }

    public class MuebleModel {
        public int MuebleID { get; set; }
        public int ServicioID { get; set; }
        public int? ServicioMuebleLocalID { get; set; }
        public string Mueble { get; set; }
    }

    public class TramoModel {
        public string Tramo { get; set; }
        public ServicioMuebleTramo Tramodata { get; set; }
        public ServicioMueble Mueble { get; set; }
        public Servicio servicio { get; set; }
        public ObservableCollection<Categoria> productos { get; set; }

    }
    public class ProductoModel {

        public string Producto { get; set; }
        public string  Conteo { get; set; }
    }



    public class ImagenPhoto {
        public byte[] photoimage { get; set; }
    }

    public class TramoFormaMueblesModel:BaseViewModel {
        public string TramoName { get; set; }
        public ObservableCollection<Nivel> niveles;
        public ObservableCollection<Nivel> Niveles { get { return niveles; } set { niveles = value;OnPropertyChanged(); } }
    }

    public class Nivel {
        public string NombreNivel { get; set; }
        public string Nivelaltura { get; set; }
        public string Nivelprofundidad { get; set; }
        public string NivelAnchura { get; set; }
    }

    public class Opcionboleana {
        public bool Activo { get; set; }
        public string DisplayName { get; set; }
    }

    public class FormaDeMuebles {
        public string Cara { get; set; }
        public string Pasillo { get; set; }
        public string Tramoinicial { get; set; }
        public int TipoDeMueble { get; set; }
        public string Altura { get; set; }
        public int? Tramos { get; set; }
        public Opcionboleana Medidasiguales { get; set; }
        public int NivelMinimo { get; set; }
        public int NivelMaximo { get; set; }
        public string Comentario { get; set; }
        public MuebleTipo TipoMueble { get; set; }
    }

    public class NIvelMedidasvalues {
        public NivelPropiedad Medida { get; set; }
        public string Valor { get; set; }
        public int Nivel { get; set; }
    }

  
   public enum NivelPropiedad {
        Profundo = 0,
        Ancho=1,
        Alto=2
    }
    public partial class ErrorLog
    {
        public int ErrorID { get; set; }
        public string Usuario { get; set; }
        public string Evento { get; set; }
        public Nullable<int> TipoID { get; set; }
        public Nullable<System.DateTime> FechaAlta { get; set; }
        public string Descripcion { get; set; }
        public object Datos { get; set; }
        public string Source { get; set; }
        public string Flujo { get; set; }
        public string Proyecto { get; set; }
        public string Parametros { get; set; }
    }
}
