using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Forms;
//using ZXing;

namespace CheckstoresMagnusRetail.Views
{
    public partial class ScannerPage : ContentPage
    {
        ServicioMuebleProductoNivel datas;
        public ScannerPage(ServicioMuebleProductoNivel data)
        {
            datas = data;
            InitializeComponent();
            Title = "Escaner";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //BarcodeScanView.IsScanning = true;
           // BarcodeScanView.IsEnabled = true;
           // TimeSpan ts = new TimeSpan(0, 0, 0, 2, 0);
           // Device.StartTimer(ts, () => {
             //   if (BarcodeScanView.IsScanning)
               //     BarcodeScanView.AutoFocus();
               // return true;
            //});
           
    
        }
/*
        public async void Handle_OnScanResult(Result result)
        {
            
                if (string.IsNullOrWhiteSpace(result.Text))
                {
                    return;
                }
                BarcodeScanView.IsScanning = false;
                BarcodeScanView.IsEnabled = false;
                datas.producto = new Producto { UPC = result.Text };

                MessagingCenter.Send<ScannerPage, string>(this, "Hi", result.Text);

         
           
        }*/
    }
}
