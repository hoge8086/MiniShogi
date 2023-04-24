using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryWithLabel : ContentView
    {
        public EntryWithLabel()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(
                nameof(LabelText), typeof(string), typeof(EntryWithLabel),
                defaultValue: string.Empty, BindingMode.TwoWay);

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly BindableProperty EntryTextProperty =
            BindableProperty.Create(
                nameof(EntryText), typeof(string), typeof(EntryWithLabel),
                defaultValue: string.Empty, BindingMode.TwoWay);

        public string EntryText
        {
            get { return (string)GetValue(EntryTextProperty); }
            set { SetValue(EntryTextProperty, value); }
        }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(
                nameof(Placeholder), typeof(string), typeof(EntryWithLabel),
                defaultValue: string.Empty, BindingMode.TwoWay);

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        [Xamarin.Forms.TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
                propertyName: nameof(FontSize),
                returnType: typeof(double),
                declaringType: typeof(EntryWithLabel),
                defaultValue: Device.GetNamedSize(NamedSize.Default,typeof(Label)),
                defaultBindingMode: BindingMode.TwoWay
            );
    }
}