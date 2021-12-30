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
    public partial class KomaView : ContentView
    {
        public KomaView()
        {
            InitializeComponent();
        }

        private void koma_SizeChanged(object sender, EventArgs e)
        {
            // [将棋の駒の形]
            var size = this.Height * 0.8;
            polygon.Points = new Xamarin.Forms.Shapes.PointCollection()
            {
                new Point(0, size),
                new Point(size, size),
                new Point(size * 0.85, size * 0.2),
                new Point(size * 0.5, 0),
                new Point(size * 0.15, size * 0.2),
            };
            polygon.HeightRequest = this.Height * 0.8;
            polygon.WidthRequest = this.Width * 0.8;
        }

        #region DisplayName
        public static readonly BindableProperty DisplayNameProperty = BindableProperty.Create(
                                                                            nameof(DisplayName),
                                                                            typeof(string),
                                                                            typeof(KomaView),
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

        #region IsRotated
        public static readonly BindableProperty IsRotatedProperty = BindableProperty.Create(
                                                                            nameof(IsRotated),
                                                                            typeof(bool),
                                                                            typeof(KomaView),
                                                                            null);
 
        /// <summary>
        /// 表示名
        /// </summary>
        public bool IsRotated
        {
            get { return (bool)GetValue(IsRotatedProperty); }
            set { SetValue(IsRotatedProperty, value); }
        }
        #endregion
    }
}