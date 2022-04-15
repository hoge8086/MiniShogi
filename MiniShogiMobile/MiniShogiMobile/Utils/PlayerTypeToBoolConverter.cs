using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MiniShogiMobile.Utils
{
    public class PlayerTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((PlayerType)value == PlayerType.Player1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? PlayerType.Player1 : PlayerType.Player2;
        }
    }
}
