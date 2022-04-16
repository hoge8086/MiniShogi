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
    public partial class TextbaseSwitch : ContentView
    {
        public TextbaseSwitch()
        {
            InitializeComponent();
            var gr = new TapGestureRecognizer();
            gr.Tapped += (s, e) =>
            {
                IsToggled = !IsToggled;
            };
            this.GestureRecognizers.Add(gr);
        }
        #region IsToggled
        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
                                                                            nameof(IsToggled),
                                                                            typeof(bool?),
                                                                            typeof(TextbaseSwitch),
                                                                            null,
                                                                            BindingMode.TwoWay);
        public bool? IsToggled
        {
            get { return (bool?)GetValue(IsToggledProperty); }
            set { SetValue(IsToggledProperty, value); }
        }
        #endregion

        #region Text
        public static readonly BindableProperty OnTextProperty = BindableProperty.Create(
                                                                            nameof(OnText),
                                                                            typeof(string),
                                                                            typeof(TextbaseSwitch),
                                                                            "ON");
        public string OnText
        {
            get { return (string)GetValue(OnTextProperty); }
            set { SetValue(OnTextProperty, value); }
        }
        public static readonly BindableProperty OffTextProperty = BindableProperty.Create(
                                                                            nameof(OffText),
                                                                            typeof(string),
                                                                            typeof(TextbaseSwitch),
                                                                            "OFF");
        public string OffText
        {
            get { return (string)GetValue(OffTextProperty); }
            set { SetValue(OffTextProperty, value); }
        }
        #endregion

        #region Color
        public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
                                                                            nameof(OnColor),
                                                                            typeof(Color),
                                                                            typeof(TextbaseSwitch),
                                                                            Color.FromHex("#46d744"),
                                                                            BindingMode.TwoWay);
        public Color OnColor
        {
            get { return (Color)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }
        public static readonly BindableProperty OffColorProperty = BindableProperty.Create(
                                                                            nameof(OffColor),
                                                                            typeof(Color),
                                                                            typeof(TextbaseSwitch),
                                                                            Color.LightGray);
                                                                            //Color.FromHex("#dd2424"));
        public Color OffColor
        {
            get { return (Color)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }
        #endregion
    }
}