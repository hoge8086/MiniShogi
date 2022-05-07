using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace MiniShogiMobile.Utils
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        private static string DisplayName(object value){
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            var fileInfo = type.GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)fileInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault();
            return descriptionAttribute.Description;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DisplayName(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
