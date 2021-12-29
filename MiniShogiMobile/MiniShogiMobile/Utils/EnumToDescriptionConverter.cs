using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace MiniShogiMobile.Utils
{
    public class EnumToDescriptionConverter<T> : IValueConverter
    {
        private static string DisplayName(T value) {
            var fileInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)fileInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault();
            return descriptionAttribute.Description;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DisplayName((T)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
