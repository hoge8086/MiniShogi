using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Utils
{
    public class EnumListProvider<T> : IMarkupExtension<IList<T>>
    {
        private static string DisplayName(T value) {
            var fileInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)fileInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault();
            return descriptionAttribute.Description;
        }

        public readonly static IList<T> EnumItems = typeof(T).GetEnumValues()
                                                        .Cast<T>()
                                                        //.Select(value => new EnumItem<T>{ Code = value, Name = DisplayName(value) })
                                                        .ToList();
        public IList<T> ProvideValue(IServiceProvider serviceProvider)
        {
            return EnumItems;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<IList<T>>).ProvideValue(serviceProvider);
        }
    }
}
