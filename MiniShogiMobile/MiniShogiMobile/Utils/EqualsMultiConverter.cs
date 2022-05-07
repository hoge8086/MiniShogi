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

            if(values[0] == null || values[1] == null)
                return false;

            if(values[0].GetType() != values[1].GetType())
                return false;


            // 値型はBoxingされていても値型として比較する
            return ((values[0] != null) && values[0].GetType().IsValueType)
                     ? values[0].Equals(values[1])
                     : (values[0] == values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
