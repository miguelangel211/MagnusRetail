using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class TiendaViewModel:BaseViewModel
    {

        private ObservableCollection<MueblesModel> mueblesentienda;
        public ObservableCollection<MueblesModel> MueblesEnTienda { get { return mueblesentienda; } set { mueblesentienda = value; OnPropertyChanged(); } }
        public Servicio servicioactual;
        private ServicioMueblesOperaciones repo = new ServicioMueblesOperaciones();
        private TramosOperaciones tramorepo = new TramosOperaciones();
        private TramoNivelesOperaciones nivelrepo = new TramoNivelesOperaciones();
        private CategoriasOperaciones caterepo = new CategoriasOperaciones();
        public bool Eliminando { get; set; }

        public TiendaViewModel(Servicio service){
            servicioactual = service;
            MueblesEnTienda = new ObservableCollection<MueblesModel>();

        }

        public override Task CargarDatos()
        {
            return cargadata();
        }

        private async Task cargadata() {
            if (IsBusy)
                return;
            IsBusy = true;

            var dataresult = await repo.ListadeMuebles(servicioactual);
            MueblesEnTienda.Clear();

            if (dataresult.realizado)
            {
                List<sqlrepo.ServicioMueble> mu = dataresult.Result;
                var muebles = new List<MueblesModel>();
                foreach (var m in mu)
                {
                    var tramosdb = await tramorepo.ListadeTRamos(m);
                    var muebledatostemp = new MueblesModel();
                    var muebletemp = new MuebleModel();
                    muebletemp.Mueble = "MUEBLE " + m.MueblePasillo +" "+m.MuebleCara ;
                    muebletemp.MuebleID = m.ServicioMuebleID??0;
                    muebletemp.ServicioMuebleLocalID = m.ServicioMuebleLocalID;
                    muebledatostemp= new MueblesModel() {
                   };
                    if (tramosdb.realizado)
                    {
                        foreach (var t in tramosdb.Result.OrderBy(x=>x.TramoNumero))
                        {
                            var categorias = await caterepo.ListadeCategoriasdelmueble(t);
                            var nivelesdb = await nivelrepo.Listadenivelestramo(t);
                            var tramotemp = new TramoModel
                            {
                                Tramodata = t,
                                Tramo = "TRAMO " + t.TramoNumero,
                                Mueble = m,
                                servicio= servicioactual,
                                productos = new System.Collections.ObjectModel.ObservableCollection<Categoria>{}
                            };
                            if (categorias.realizado)
                            {
                                foreach (var c in categorias.Result)
                                {
                                    tramotemp.productos.Add(c);
                                }
                            }
                           
                            muebledatostemp.Add(tramotemp);
                            muebledatostemp.Tramosstorage.Add(tramotemp);
                           
                        }
                    }
                    
                    muebledatostemp.Mueble = muebletemp;
                    muebledatostemp.Expanded = true;
                    muebles.Add(muebledatostemp);
                }
                
               muebles= muebles.OrderBy(x=>x.Mueble.Mueble).ToList();
                
                MueblesEnTienda = new ObservableCollection<MueblesModel>(muebles);            }
            else
            {
                await mensajetoast(dataresult.Errores);
            }
           
          IsBusy = false;

        }

        public async Task EliminarMueble(MuebleModel mueble) {
           await repo.EliminarMueble(mueble);
            CargarData.Execute(null);
        }

    }
}
