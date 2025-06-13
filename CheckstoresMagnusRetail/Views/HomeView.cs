using System;

using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public class HomeView : ContentPage
    {
        public HomeView()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

