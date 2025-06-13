using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.RemoteConfig;
using Foundation;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.XForms.iOS.EffectsView;
using Syncfusion.XForms.iOS.MaskedEdit;
using UIKit;

namespace CheckstoresMagnusRetail.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            new Syncfusion.XForms.iOS.ComboBox.SfComboBoxRenderer();

            Xamarin.Forms.Forms.Init();
            XF.Material.iOS.Material.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();



            this.LoadApplication(new App());
            GoogleVisionBarCodeScanner.iOS.Initializer.Init();
            // Temporary work around for bug on Firebase Library
            // https://github.com/xamarin/GoogleApisForiOSComponents/issues/368
            Firebase.Core.App.Configure();
            RemoteConfig.SharedInstance.ConfigSettings = new RemoteConfigSettings();
            Syncfusion.XForms.iOS.Border.SfBorderRenderer.Init();
            Syncfusion.XForms.iOS.Buttons.SfButtonRenderer.Init();
            SfListViewRenderer.Init();
            SfEffectsViewRenderer.Init();
            SfMaskedEditRenderer.Init();
            UIBarButtonItem.Appearance.SetTitlePositionAdjustment(new UIOffset(20, 0), UIBarMetrics.Compact);


            return base.FinishedLaunching(app, options);
        }
    }
}
