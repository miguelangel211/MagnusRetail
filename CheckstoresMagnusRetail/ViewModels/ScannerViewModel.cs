using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.Views;
//using Rb.Forms.Barcode.Pcl;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class ScannerViewModel : BaseViewModel
    {
        private String barcode = "";
        private bool initialized = false;
        private bool preview = true;
        private bool torch = false;
        private bool decoder = true;
        public bool activo = true;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand PreviewActivatedCommand { get; private set; }
        public ICommand BarcodeChangedCommand { get; private set; }
        public ICommand BarcodeDecodedCommand { get; private set; }
        public ICommand TogglePreviewCommand { get; private set; }
        public ICommand ToggleTorchCommand { get; private set; }
        public ICommand ToggleDecoderCommand { get; private set; }

/*

        public String Barcode
        {
            get { return barcode; }
            set
            {
                barcode = value;
                OnPropertyChanged();
            }
        }

        public bool Initialized
        {
            get { return initialized; }
            set
            {
                initialized = value;
                OnPropertyChanged();
            }
        }
        public bool Activo
        {
            get { return activo; }
            set
            {
                activo = value;
                OnPropertyChanged();
            }
        }

        

        public bool Preview
        {
            get { return preview; }
            set
            {
                preview = value;
                OnPropertyChanged();
            }
        }

        public bool Torch
        {
            get { return torch; }
            set
            {
                torch = value;
                OnPropertyChanged();
            }
        }


        public bool Decoder
        {
            get { return decoder; }
            set
            {
                decoder = value;
                OnPropertyChanged();
            }
        }
        public TramoModel Tramo;
        public Categoria Categorias;
        public ScannerViewModel(TramoModel tramo,Categoria categoria)
        {
            Categorias = categoria;
            Tramo = tramo;
            PreviewActivatedCommand = new Command(() => { Initialized = true; });
            BarcodeChangedCommand = new Command<Barcode>(updateBarcode);
            BarcodeDecodedCommand = new Command<Barcode>(logBarcode);
            TogglePreviewCommand = new Command(() => { Preview = !Preview; });
            ToggleTorchCommand = new Command(() => { Torch = !Torch; });
            ToggleDecoderCommand = new Command(() => { Decoder = !Decoder; });
        }

        private void logBarcode(Barcode barcode)
        {
            Decoder = false;

            if (IsBusy)
                return;
            IsBusy = true;
            if (Activo)
            {
                Activo = false;

                Debug.WriteLine("Decoded barcode [{0} - {1}]", barcode?.Result, barcode?.Format);


                Barcode = String.Format("UPC: {0}", barcode?.Result, barcode?.Format);
                // MessagingCenter.Send(this, "Hi", barcode?.Result);
                var d = new NavigationPage(new AgregarProductoPage(new ServicioMuebleProductoNivel {
                    tramo = Tramo.Tramodata, producto = new Producto { UPC = barcode?.Result } }, false, Categorias));

                d.Style = (Style)Xamarin.Forms.Application.Current.Resources["SecondaryPage"];
                try
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Navigation.PushModalAsync(d);
                    });
                }
                catch { }
            }
            Barcode = "";
            IsBusy = false;
        }

        private void updateBarcode(Barcode barcode)
        {

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }*/
    }
}
