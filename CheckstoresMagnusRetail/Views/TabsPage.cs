using System;

using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public class TabsPage : ContentPage
    {
        public TabsPage()
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

