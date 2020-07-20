using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class LogModel:BaseViewModel
    {
        public List<string> Datos { get; set; }
        public ObservableCollection<string> data;
        public ObservableCollection<string> Datalist { get { return data; } set { data = value;OnPropertyChanged(); } }


        public Command primercarga;
        public Command Primercarga { get { return primercarga; } set { primercarga = value;OnPropertyChanged(); } }

        public Command cargarmas;
        public Command CargarMas { get { return primercarga; } set { primercarga = value; OnPropertyChanged(); } }



        public LogModel(List<string>datos)
        {
            IsBusy = false;

            Datos = datos;
            Datos.Reverse();
            Datalist = new ObservableCollection<string>();
            Primercarga = new Command(async()=>await Cargardatap());
            CargarMas = new Command(async () => await Cragarmasdatos());
        }


        public async Task Cargardatap() {
            if (IsBusy)
                return;
            IsBusy = true;
            Datalist = new ObservableCollection<string>(Datos.Take(100));
            IsBusy = false;
        }

        public async Task Cragarmasdatos()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            var nuevos= (Datos.Skip(Datalist.Count).Take(100));
            
            foreach (var n in nuevos) {
                Datalist.Add(n);
            }
            IsBusy = false;
        }
    }
}
