using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// 参考:<https://www.gunshi.info/entry/2018/09/04/172058>
namespace MiniShogiMobile.Utils
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string))
            {
                return null;
            }

            //以下だとUWPでエラーになる。
            //return ImageSource.FromResource(value.ToString());

            var assembly = typeof(App).GetTypeInfo().Assembly;
            return ImageSource.FromResource(value.ToString(), assembly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    [ContentProperty(nameof(Source))]
    class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            var assembly = typeof(App).GetTypeInfo().Assembly;
            return ImageSource.FromResource(Source, assembly);
        }
    }

}
