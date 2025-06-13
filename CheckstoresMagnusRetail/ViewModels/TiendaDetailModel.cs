using System;
namespace CheckstoresMagnusRetail.ViewModels
{
    public class TiendaDetailModel:BaseViewModel
    {
        public ObservableCollection<MueblesModel> mueblesentienda;
        public ObservableCollection<MueblesModel> MueblesEnTienda { get { return mueblesentienda; } set { mueblesentienda = value; OnPropertyChanged(); } }
        public TiendaViewModel()
        {
            mueblesentienda = new ObservableCollection<MueblesModel>();

        }


        public TiendaDetailModel()
        {
        }
    }
}
