using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{


    public class photoobject : INotifyPropertyChanged
    {
        #region Fields

        private byte[] image;
        private string nombre;
        public int IDLocal;
        #endregion

        #region Public Properties


        public byte[] ContactImage
        {
            get { return this.image; }
            set
            {
                this.image = value;
                this.RaisePropertyChanged("ContactImage");
            }
        }

        public string Nombre
        {
            get { return this.nombre; }
            set
            {
                this.nombre = value;
                this.RaisePropertyChanged("Nombre");
            }
        }
        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
