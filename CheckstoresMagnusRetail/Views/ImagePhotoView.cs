using System;

using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Views
{
    public class ImagePhotoView : ContentPage
    {
        public ImagePhotoView()
        {
            Content = new StackLayout
            {
                Children = {
                    new Image{Source = "lock.png" }
                }
            };
        }
    }
}

