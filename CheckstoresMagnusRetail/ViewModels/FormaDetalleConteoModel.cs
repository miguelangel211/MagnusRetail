using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class FormaDetalleConteoModel:BaseViewModel
    {
        public ObservableCollection<ServicioMuebleProductoNivel> productos;
        public ObservableCollection<ServicioMuebleProductoNivel> Prodcutos { get { return productos; } set { productos = value;OnPropertyChanged(); } }
        ProductoOperacion productooperaciones = new ProductoOperacion();
        TramoModel Parameter;
        ServicioProductoNivelOperaciones serrepo;
        Categoria categoriadelnivel;
        public FormaDetalleConteoModel(TramoModel parameter,Categoria parametrocategoria)
        {
            Prodcutos = new ObservableCollection<ServicioMuebleProductoNivel>();
            Parameter = parameter;
            categoriadelnivel = parametrocategoria;
            serrepo = new ServicioProductoNivelOperaciones();
        }

        public override Task CargarDatos()
        {
            return  Datos();
        }

        public async Task Datos() {
            if (IsBusy)
                return;

            IsBusy = true;
            productos.Clear();
            Parameter.Tramodata.Categoria = categoriadelnivel;
            var productospornivel = await serrepo.consultarListadodedata(Parameter.Tramodata);
           // var datosproductos = await productooperaciones.consultarListadodedata(new ServicioMuebleTramo { });
            if (productospornivel.realizado)
            {
                foreach (var p in productospornivel.Result) {
                    Prodcutos.Add(p);
                }
            }
            IsBusy = false;
        }


    }
}
