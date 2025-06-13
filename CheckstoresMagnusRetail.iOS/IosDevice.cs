using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckstoresMagnusRetail.iOS;
using CheckstoresMagnusRetail.sqlrepo;
using Foundation;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(IosDevice))]
namespace CheckstoresMagnusRetail.iOS
{
    class IosDevice : IDevice
    {
        public string GetIdentifier()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        }
    }
}