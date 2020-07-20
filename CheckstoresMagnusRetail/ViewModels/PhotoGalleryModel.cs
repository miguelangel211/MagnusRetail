
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.Views;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class PhotoGalleryModel:BaseViewModel
    {
        public ObservableCollection<photoobject> listadeImagenes;

        public ObservableCollection<photoobject> ListadeImagenes { get { return listadeImagenes; } set { listadeImagenes = value;OnPropertyChanged(); } }
        public byte[] fotoactual;
        public byte[] FotoActual { get {
                return fotoactual;
            } set {
                SetProperty(ref fotoactual, value);
                // fotoactual = value;OnPropertyChanged("FotoActual");
            } }
        ProductoOperacion prop;
        object FOTOOP;
        int tgaleriatype;
        public Command tomarfoto;
        public Command TomarFoto { get { return tomarfoto; } set { tomarfoto = value;OnPropertyChanged(); } }
        object origentype;
        public int  IDImagen { get; set; }
        public Command fotoseleccionada;
        public Command Fotoseleccionada { get { return fotoseleccionada; }set { fotoseleccionada = value;OnPropertyChanged(); } }
        public  PhotoGalleryModel(int tipogaleria,object fuente)
        {
            origentype = fuente;
            ListadeImagenes = new ObservableCollection<photoobject>();
            TomarFoto = new Command(async()=> await tomarfotot());
            prop = new ProductoOperacion();
            tgaleriatype = tipogaleria;
            switch (tipogaleria) {

                case 1: FOTOOP = new muebleFotgrafiasOperaciones();break;
                case 2: FOTOOP = new LayoutFotosOperaciones();break;
                case 3: FOTOOP = new FotoProductoOperaciones();break;
            }
           // Fotoseleccionada = new Command<object>(async(object o)=>await tappedphoto(o));
            //carga();
        }

        public override Task CargarDatos()
        {
           
            switch (tgaleriatype)
            {

                case 1: return cargafotosmueble();
                case 2: return cargafotoslayout();
                case 3: return cargafotosproducto();
                default: return cargafotosmueble();

            }

        }

        public async Task cargafotosmueble() {
            var datos =await (FOTOOP as muebleFotgrafiasOperaciones).consultarListadodedata(origentype);
            if (datos.realizado)
            {
                foreach (var d in datos.Result)
                {
                    ListadeImagenes.Add(new photoobject { ContactImage = d.MuebleImagen,IDLocal=d.ServicioMuebleImagenLocalID??0 });
                }
                IDImagen = ListadeImagenes.LastOrDefault().IDLocal;
                FotoActual = ListadeImagenes.LastOrDefault().ContactImage;

            }
        }

        public async Task cargafotoslayout()
        {
            var datos = await (FOTOOP as LayoutFotosOperaciones).consultarListadodedata(origentype);
            if (datos.realizado)
            {
                foreach (var d in datos.Result)
                {
                    ListadeImagenes.Add(new photoobject { ContactImage = d.LayoutImagen,IDLocal=d.ServicioLayoutLocalID??0 });
                }
                FotoActual = ListadeImagenes.LastOrDefault().ContactImage;
                IDImagen = ListadeImagenes.LastOrDefault().IDLocal;

            }
        }


        public async Task cargafotosproducto()
        {
            var datos = await (FOTOOP as FotoProductoOperaciones).consultarListadodedata(origentype);
            if (datos.realizado)
            {
                foreach (var d in datos.Result)
                {
                    ListadeImagenes.Add(new photoobject { ContactImage = d.ProductoImagen1,IDLocal=d.ProductoImagenLocalID ?? 0 });
                }
                FotoActual = ListadeImagenes.LastOrDefault().ContactImage;
                IDImagen = ListadeImagenes.LastOrDefault().IDLocal;

            }
        }


        public async Task tomarfotot() {
            if (IsBusy)
                return;
            IsBusy = true;
            MediaFile photo;
            if (tgaleriatype == 3)
            {
                photo = await Plugin.Media.CrossMedia.Current.
                TakePhotoAsync(
                new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                {
                    CompressionQuality = 50,
                    PhotoSize = PhotoSize.Medium

                }) ;
            }
            else {
                photo = await Plugin.Media.CrossMedia.Current.
               TakePhotoAsync(
               new Plugin.Media.Abstractions.StoreCameraMediaOptions()
               {
                   CompressionQuality = 50,
                   PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
               });
            }
            if (photo != null)
            {
                try
                {
                    var photoob = new photoobject();
                    FotoActual = File.ReadAllBytes(photo.Path);

                    photoob.ContactImage = FotoActual;
                    string extension = ".jpg";
                    string filename = Path.ChangeExtension(Path.GetRandomFileName(), extension);
                    File.Delete(photo.Path);
                    switch (tgaleriatype)
                    {
                        case 1:
                            var im = new ServicioMuebleImagen
                            {
                                MuebleImagen = FotoActual,
                                ServicioMuebleID = (origentype as ServicioMueble).ServicioMuebleID,
                                MuebleImagenRuta = filename,
                                ServicioMuebleLocalID = (origentype as ServicioMueble).ServicioMuebleLocalID ?? 0,
                                FechaHoraLocal = DateTime.Now,
                                ServicioID = (origentype as ServicioMueble).ServicioID
                            };
                            await (FOTOOP as muebleFotgrafiasOperaciones).insertarregistro(im);
                            IDImagen = im.ServicioMuebleImagenLocalID??0;
                            break;
                        case 2:
                            var foto = new ServicioLayout
                            {
                                LayoutImagen = FotoActual,
                                ServicioID = (origentype as Servicio).ServicioID,
                                FechaHoraLocal = DateTime.Now,
                                LayoutRuta = filename
                            };
                            await (FOTOOP as LayoutFotosOperaciones).insertarregistro(foto);
                            IDImagen = foto.ServicioLayoutLocalID ?? 0;
                            break;
                        case 3:
                            var imagen = new ProductoImagen { ProductoImagen1 = FotoActual, ProductoID = (origentype as Producto).ProductoID, ProductoLocalID = (origentype as Producto).ProductoLocalID, FechaHoraLocal = DateTime.Now, ProductImageRuta = filename };
                            await (FOTOOP as FotoProductoOperaciones).insertarregistro(imagen);
                            IDImagen = imagen.ProductoImagenLocalID ?? 0;
                            break;

                    }
                    photoob.IDLocal = IDImagen;
                    ListadeImagenes.Add(photoob);

                }
                catch { }
                IsBusy = false;
            }
        }

        public async void EliminarFoto() {
            if (ListadeImagenes.Count>0) { 
            var imagen = ListadeImagenes.FirstOrDefault(x => x.IDLocal == IDImagen);
            ListadeImagenes.Remove(imagen);

                switch (tgaleriatype)
                {
                    case 1:
                        await (FOTOOP as muebleFotgrafiasOperaciones).borrarfoto(IDImagen);
                        break;
                    case 2:
                        await (FOTOOP as LayoutFotosOperaciones).borrarfoto(IDImagen);
                        break;
                    case 3:
                        await (FOTOOP as FotoProductoOperaciones).borrarfoto(IDImagen);
                        break;
                }
            }
            FotoActual = null;
            if (ListadeImagenes.Count > 0)
            {
                var nuevaultimaimagen = ListadeImagenes.LastOrDefault();
                FotoActual = nuevaultimaimagen.ContactImage;
                IDImagen = nuevaultimaimagen.IDLocal;
            }
          
        }

    }
}
