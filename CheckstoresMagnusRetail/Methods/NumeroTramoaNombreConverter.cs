using System;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Methods
{

    public class NumeroTramoaNombreConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string retSource = null;
            if (value != null)
            {
                int numerotramo = (int)value;
                retSource = "Tramo " + numerotramo;
            }
            return retSource;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
