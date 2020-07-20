using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
//using Plugin;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using CheckstoresMagnusRetail.DataModels;
//using Rb.Forms.Barcode.Droid;

namespace CheckstoresMagnusRetail.Droid
{
    [Activity(Label = "Checkstores MagnusRetail", Icon = "@drawable/iconoapp", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            //ZXing.Net.Mobile.Forms.Android.Platform.Init();


          //  CrossCurrentActivity.Current.Init(this, savedInstanceState);
            GoogleVisionBarCodeScanner.Droid.RendererInitializer.Init();


            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            /*  var config = new Configuration
              {
                  Zoom = 5
              };*/

            // BarcodeScannerRenderer.Init(config);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
           // Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                //const string errorFileName = "Fatal.log";
                //var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                //var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
                DateTime.Now, exception.ToString());
                //  File.WriteAllText(errorFilePath, errorMessage);
                escribirerror(errorMessage, exception.StackTrace);
                Android.Util.Log.Error("Crash Report", errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }


        public static void escribirerror(string datos, string stack)
        {
            string fileName = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
      DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt");
            using (StreamWriter sw = (File.Exists(fileName)) ? File.AppendText(fileName) : File.CreateText(fileName))
            {
                // var parametros = JsonConvert.SerializeObject(param);

                var errorstring = JsonConvert.SerializeObject(new ErrorLog
                {
                    Usuario = "0",
                    FechaAlta = DateTime.Now,
                    Descripcion = "Error App Checkstore",
                    Proyecto = "Checkstore MOBILEAPP",
                    TipoID = 2,
                    Evento = "global error",
                    Datos = datos,
                    Parametros = "",
                    Flujo = "error global no handled exception",
                    Source = stack
                });

                // var errorstring= 
                sw.WriteLine(errorstring);
            }
        }
    }
    
}