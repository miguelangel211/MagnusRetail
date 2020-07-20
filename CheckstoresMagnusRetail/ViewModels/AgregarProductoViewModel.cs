using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class AgregarProductoViewModel:BaseViewModel
    {
        ProductoOperacion op;
        public ObservableCollection<ServicioMuebleProductoNivel> niveles;
        public ObservableCollection<ServicioMuebleProductoNivel> Niveles { get { return niveles; } set { niveles = value;OnPropertyChanged(); } }
        public Command buscarproducto;
        public Command BuscarProducto { get { return buscarproducto; }set { buscarproducto = value;OnPropertyChanged(); } }

        private Command visualizarproductodata;
        public Command VisualizarProductoDatos { get { return visualizarproductodata; } set { visualizarproductodata = value;OnPropertyChanged(); } }
        public Producto datosforma;

        public Producto DatosForma { get { return datosforma; } set { datosforma = value;OnPropertyChanged(); } }
        TramoNivelesOperaciones niverepo = new TramoNivelesOperaciones();
        ServicioProductoNivelOperaciones prodnivrepo;
        public bool upcencontrado;
        public bool nuevoregistro;
        public bool UPCencontrado { get { return upcencontrado; } set { upcencontrado = value;OnPropertyChanged(); } }
        ServicioMuebleProductoNivel datosniveltramo;
        ServicioMueblesOperaciones mueblerepo;
        private bool seccionproductovisibilidad;
        public bool SeccionProductoVisibilidad { get { return seccionproductovisibilidad; } set { seccionproductovisibilidad = value;OnPropertyChanged(); } }

        private int rowhegith;
        public int AlturaProductocontrol { get { return rowhegith; } set { rowhegith = value;OnPropertyChanged(); } }

        private int levelsheigth;
        public int Alturalistaniveles { get { return levelsheigth; } set { levelsheigth = value;OnPropertyChanged(); } }

        CategoriasOperaciones catrepo = new CategoriasOperaciones();

        Categoria categoriadelnivel;
        bool modof;
       public bool Modoforma { get { return modof; } set { modof = value;OnPropertyChanged(); } }

        private ObservableCollection<Categoria> categorias;
        public ObservableCollection<Categoria> Categorias { get { return categorias; } set { categorias = value; OnPropertyChanged(); } }

        private double maximaaltura;
        public AgregarProductoViewModel(ServicioMuebleProductoNivel datos,bool editando,Categoria parametercategoria)
        {
            Modoforma = editando;
            datosniveltramo = datos;
            categoriadelnivel = parametercategoria;
            mueblerepo = new ServicioMueblesOperaciones();
            prodnivrepo = new ServicioProductoNivelOperaciones();
            Niveles = new ObservableCollection<ServicioMuebleProductoNivel>();
            op = new ProductoOperacion();
            DatosForma = new Producto();
            nuevoregistro = false;
            BuscarProducto = new Command<string>(async(string m)=> await buscarproduct(m));
            VisualizarProductoDatos = new Command(async () => await visualizacionproductoseccion());
           Alturalistaniveles = 200;
            AlturaProductocontrol = 60;
           maximaaltura= DeviceDisplay.MainDisplayInfo.Height;
            SeccionProductoVisibilidad = true;
        }


        private async Task visualizacionproductoseccion() {
            if (seccionproductovisibilidad)
            {
                AlturaProductocontrol = 0;
                double alturatemp=
                 (Niveles.Count * 60)+20;

                if (alturatemp > (maximaaltura - 1050)) 
                    alturatemp=( maximaaltura - 1050);
                Alturalistaniveles =(int)alturatemp;
                

            }
            else {
                AlturaProductocontrol = 60;
                Alturalistaniveles = 200;
            }
            SeccionProductoVisibilidad = !SeccionProductoVisibilidad;

        }
        private async Task buscarproduct(string h) {
        // var prods=  await op.Listadeproductos();
            var prod = await op.consultarDatoconcisoAsync(DatosForma);
            if (prod.realizado)
            {
                DatosForma = prod.Result;
                nuevoregistro= false;
            }
            else {
                nuevoregistro = true;
                if (DatosForma.ProductoLocalID.HasValue) {
                    DatosForma = new Producto { UPC = h };
                }

            }
        }

        public async Task<bool> checarniveles()
        {
            foreach (var nivel in Niveles)
            {
                if (nivel.Profundo == null || nivel.Posicion == null || nivel.Frente == null) {
                    await mensajetoast("Complete todos los campos del " +nivel.NombreNivel);

                    return true;
                }
            }

            return false;
        }

            public async Task<bool> guardarproducto() {
            if (DatosForma is null)
                return false;
            /*
            if ((DatosForma.UPC != null && DatosForma.UPC != "") &&
                (DatosForma.Nombre != null && DatosForma.Nombre != "") &&
                (DatosForma.Marca != null && DatosForma.Marca != "") &&
                (DatosForma.Fabricante != null && DatosForma.Fabricante != "") &&
                (DatosForma.Gramaje != null && DatosForma.Gramaje != "") &&
                (DatosForma.Alto != null && DatosForma.Alto != "") &&
                (DatosForma.Ancho != null && DatosForma.Profundo != "")&&
                (DatosForma.CategoriaID != null)
                )*/

            if(!string.IsNullOrEmpty(DatosForma.UPC) &&
                !string.IsNullOrEmpty(DatosForma.Nombre)&&
                !string.IsNullOrEmpty(DatosForma.Marca)&&
                !string.IsNullOrEmpty(DatosForma.Fabricante)&&
                !string.IsNullOrEmpty(DatosForma.Gramaje)&&
                !string.IsNullOrEmpty(DatosForma.Alto)&&
                !string.IsNullOrEmpty(DatosForma.Ancho)&&
                DatosForma.CategoriaID!=null)
                {
                var checa =await checarniveles();
                if (checa)
                    return false;
                

                if (nuevoregistro)
                {
                    if (!await op.verificarminimodefotos(DatosForma))
                        return false;
                    
                    await op.insertarregistro(DatosForma);
                    await op.guardarimagenesdelproducto(DatosForma);
                }
           

                    foreach (var nivel in Niveles)
                    {
                    
                    nivel.ProductoID = DatosForma.ProductoID;
                    nivel.ProductoLocalID = DatosForma.ProductoLocalID.Value;
                    nivel.CategoriaID = categoriadelnivel.CategoriaID ?? 0;
                    nivel.CategoriaLocalID = categoriadelnivel.CategoriaLocalID ?? 0 ;
                        await prodnivrepo.guardarregistroproductonivel(nivel);
                    }
                   //  MessagingCenter.Send(this, "Hi", "actualizar");
                return true;
                
            }
            else {
                await mensajetoast("Complete los campos del producto");

                return false;
            }
        }
        public override Task CargarDatos()
        {
            return carga();
        }

        private async Task carga() {
            Niveles.Clear();
            resultfromLocalDB<List<ServicioMuebleProductoNivel>> nivelesproductovalues;
            var nivelesbd = await niverepo.Listadenivelestramo(datosniveltramo.tramo);
            if (Modoforma)
            {
                nivelesproductovalues = await prodnivrepo.Listadenivelesproducto(datosniveltramo.tramo,DatosForma);
            }
            else {
                nivelesproductovalues = new resultfromLocalDB<List<ServicioMuebleProductoNivel>>() { realizado = false };
            }

            var categoriaslist = await catrepo.ListadeCategorias();
            if (categoriaslist.realizado)
                Categorias = new ObservableCollection<Categoria>(categoriaslist.Result);

            var mueble =await mueblerepo.consultarDatoconcisoAsync(new ServicioMueble {ServicioMuebleID=datosniveltramo.tramo.ServicioMuebleID,ServicioMuebleLocalID=datosniveltramo.tramo.ServicioMuebleLocalID });
            int index = 0;
            int Diferencia = (mueble.Result.MuebleNivelMaximo??0 - mueble.Result.MuebleNivelMinimo??0)+1;
                for (int i = mueble.Result.MuebleNivelMinimo??0; i <= (mueble.Result.MuebleNivelMaximo??0); i++) {

                    if (nivelesbd.Result.Count > index)
                    {
                        ServicioMuebleProductoNivel desdebdcreado;
                        if (nivelesproductovalues.realizado)
                        {
                            desdebdcreado = nivelesproductovalues.Result.FirstOrDefault(x => (x.ServicioMuebleTramoNivelID == (nivelesbd.Result[index] ?? new ServicioMuebleTramoNivel()).ServicioMuebleTramoNivelID && (nivelesbd.Result[index] ?? new ServicioMuebleTramoNivel()).ServicioMuebleTramoNivelID != 0) ||
                            (x.ServicioMuebleTramoNivelLocalID == (nivelesbd.Result[index] ?? new ServicioMuebleTramoNivel()).ServicioMuebleTramoNivelLocalID && (nivelesbd.Result[index] ?? new ServicioMuebleTramoNivel()).ServicioMuebleTramoNivelLocalID != 0));
                        }
                        else {
                            desdebdcreado = null;
                        }
                        if (desdebdcreado != null)
                        {
                            desdebdcreado.NombreNivel = "Nivel " + i;
                        desdebdcreado.Editable = Modoforma;
                            Niveles.Add(desdebdcreado);
                        }
                        else
                        {
                        Niveles.Add(new ServicioMuebleProductoNivel
                        { NombreNivel = "Nivel " + i,
                            ServicioMuebleTramoNivelID = (int)nivelesbd.Result[index].ServicioMuebleTramoNivelID,
                            Editable = Modoforma,
                            Profundo=0,
                            Frente=0,
                            Posicion=0,
                                ServicioMuebleTramoNivelLocalID = (int)nivelesbd.Result[index].ServicioMuebleTramoNivelLocalID }); ;
                        }
                    }
                    index++;
                }
            }

        
    
        public async Task mensajetoast(string Error)
        {
            var color = new XF.Material.Forms.UI.Dialogs.Configurations.MaterialSnackbarConfiguration();
            color.BackgroundColor = (Color)Application.Current.Resources["azul"];
            await MaterialDialog.Instance.SnackbarAsync(message: Error,
            msDuration: 3000, color);
        }
    }
}
