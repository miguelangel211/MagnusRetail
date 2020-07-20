using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ViewModels;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public partial class LogPage : ContentPage
    {

        LogModel context;
        public LogPage(List<string> logtext)
        {
            InitializeComponent();
            this.BindingContext = context = new LogModel(logtext);
           
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            await Task.Delay(200);
            context.Primercarga.Execute(null);
     
           
        }
    }
}
