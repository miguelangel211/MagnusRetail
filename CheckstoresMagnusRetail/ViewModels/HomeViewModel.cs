using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using CheckstoresMagnusRetail.DataModels;
using Xamarin.Forms;
using System.Linq;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Essentials;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class HomeViewModel:BaseViewModel
    {
        public string fecha;
        public string Fecha { get { return fecha; } set { fecha = value; OnPropertyChanged(); } }
        public string fechatexto;
        public int numeropendientes;
        public int NumeroPendientes { get { return numeropendientes; } set { numeropendientes = value;OnPropertyChanged(); } }
        public string FechaTexto { get { return fechatexto; } set { fechatexto = value;OnPropertyChanged(); } }
        private ObservableCollection<TiendaModel> tiendas;
        public ObservableCollection<TiendaModel> Tiendas { get { return tiendas; } set { tiendas = value;OnPropertyChanged(); } }
        private ServiciosOperaciones repo = new ServiciosOperaciones();
    


        public HomeViewModel()
        {
            CultureInfo ci = new CultureInfo("Es-Es");
            SincronizacionData = false;
            NumeroPendientes = 0;
            Tiendas = new ObservableCollection<TiendaModel>();
            Fecha = DateTime.Now.Day + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Year;
            FechaTexto = DateTime.Now.Day +" "+ ci.DateTimeFormat.GetMonthName(DateTime.Now.Month)+ " de " +DateTime.Now.Year;
            //ci.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek) +" " +
        }

        public override Task CargarDatos()
        {
            return CargadedatosAPI();
        }



        private async Task CargadedatosAPI() {
            if (IsBusy)
                return;

            IsBusy = true;
            List<sqlrepo.Servicio> servicios;
            var dataresult = await repo.ListadeServiciosporusuario();
            Tiendas.Clear();

            if (dataresult.realizado)
            {
                if (dataresult.Result != null)
                {
                    servicios = dataresult.Result;
                    NumeroPendientes = servicios.Count;

                    List<TiendaDataModel> stores = servicios.Select(x => new TiendaDataModel
                    {
                        NombreCadena = x.NombreCadena,
                        NombreTienda = x.NombreTienda,
                        Ubicacion = x.Ubicacion
                    }).ToList();
                    stores = stores.Distinct().ToList();
                    foreach (var store in stores)
                    {
                        var tiendatemp = new TiendaModel();

                        var serviciosdetienda = servicios.Where(x => x.NombreTienda == store.NombreTienda).OrderBy(x=>x.ServicioID);
                        tiendatemp.Heading = store;
                        foreach (var s in serviciosdetienda)
                        {
                            tiendatemp.Add(s);
                            tiendatemp.Serviciosstorage.Add(s);
                            tiendatemp.Expanded = true;
                        }
                        
                        Tiendas.Add(tiendatemp);


                    }
                   
                }
            }
            else
            {
                await mensajetoast(dataresult.Errores);
            }
            IsBusy = false;
        }
       

    }
}
