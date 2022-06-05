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
    public partial class HandKomaView : ContentView
    {
        public HandKomaView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty NumberProperty =
            BindableProperty.Create(
                nameof(Number), typeof(int), typeof(HandKomaView), 0);

        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        #region DisplayName
        public static readonly BindableProperty DisplayNameProperty = BindableProperty.Create(
                                                                            nameof(DisplayName),
                                                                            typeof(string),
                                                                            typeof(HandKomaView),
                                                                            null);
 
        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }
        #endregion

        public static readonly BindableProperty IsHighlightProperty =
            BindableProperty.Create(
                nameof(IsHighlight), typeof(bool), typeof(HandKomaView), false);

        public bool IsHighlight
        {
            get { return (bool)GetValue(IsHighlightProperty); }
            set { SetValue(IsHighlightProperty, value); }
        }


        #region IsRotated
        public static readonly BindableProperty IsRotatedProperty = BindableProperty.Create(
                                                                            nameof(IsRotated),
                                                                            typeof(bool),
                                                                            typeof(HandKomaView),
                                                                            null);
 
        /// <summary>
        /// 反転
        /// </summary>
        public bool IsRotated
        {
            get { return (bool)GetValue(IsRotatedProperty); }
            set { SetValue(IsRotatedProperty, value); }
        }
        #endregion
        public KomaView GetKoma()
        {
            return koma;
        }
    }

}