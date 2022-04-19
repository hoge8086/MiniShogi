using System;
using System.Globalization;
using Xamarin.Forms;

namespace MiniShogiMobile.Utils
{
    class EqualsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return false;

            return (values[0] == values[1]) || (values[0] == null && values[1] == null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
