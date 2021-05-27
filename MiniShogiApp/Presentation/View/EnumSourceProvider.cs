using MiniShogiApp.Presentation.ViewModel;
using Shogi.Business.Domain.Model.GameFactorys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;

// 参考:<https://qiita.com/flasksrw/items/f3bd8153c32dbcdfc7fb>
namespace MiniShogiApp.Presentation.View
{
    public class EnumSourceProvider<T> : MarkupExtension {
        private static string DisplayName(T value) {
            var fileInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute)fileInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault();
            return descriptionAttribute.Description;
        }

        public IEnumerable Source { get; } 
            = typeof(T).GetEnumValues()
                .Cast<T>()
                .Select(value => new { Code = value, Name = DisplayName(value) });

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class EnumPlayerTypeSourceProvider : EnumSourceProvider<PlayerType> { }
    public class EnumGameTypeSourceProvider : EnumSourceProvider<GameType> { }
}
