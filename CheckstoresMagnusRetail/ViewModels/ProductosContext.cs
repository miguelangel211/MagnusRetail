using System;
using System.Collections.ObjectModel;
using CheckstoresMagnusRetail.DataModels;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class ProductosContext:BaseViewModel
    {

        public  ObservableCollection<ProductoModel> productos;
        public  ObservableCollection<ProductoModel> Productos { get { return productos; } set { productos = value; OnPropertyChanged(); } }
        public ProductosContext()
        {
            Productos = new ObservableCollection<ProductoModel>();
        }
    }
}
