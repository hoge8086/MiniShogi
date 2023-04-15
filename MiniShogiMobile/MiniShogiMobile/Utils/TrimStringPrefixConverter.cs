using System;
using Xamarin.Forms;

// 参考:<https://www.gunshi.info/entry/2018/09/04/172058>
namespace MiniShogiMobile.Utils
{
    public class TrimStringPrefixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value is string str) && (parameter is string strLen) && int.TryParse(strLen, out int len) && (len > 0))
            {
                if (len < str.Length)
                    return str.Substring(0, len);
                else
                    return str;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
