using System;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.Methods
{

    public class NUmeroNivelConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string retSource = null;
            if (value != null)
            {
                int numeronivel = (int)value;
                retSource = "Nivel " + numeronivel;
            }
            return retSource;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
