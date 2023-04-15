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

        public KomaView(KomaView koma)
        {
            InitializeComponent();
            this.DisplayName = koma.DisplayName;
            this.IsRotated = koma.IsRotated;
            this.IsPromoted = koma.IsPromoted;
            this.HeightRequest = koma.Height;
            this.WidthRequest = koma.Width;
        }

        private void koma_SizeChanged(object sender, EventArgs e)
        {
            // [将棋の駒の形]
            var size = (this.Height < this.Width ? this.Height : this.Width);// * 0.8;
            polygon.Points = new Xamarin.Forms.Shapes.PointCollection()
            {
                new Point(0, size),
                new Point(size-1, size),
                new Point(size * 0.85-1, size * 0.2),
                new Point(size * 0.5-1, 0),
                new Point(size * 0.15-1, size * 0.2),
            };
            polygon.HeightRequest = size;//this.Height * 0.8;
            polygon.WidthRequest = size;// this.Width * 0.8;
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
        /// 反転
        /// </summary>
        public bool IsRotated
        {
            get { return (bool)GetValue(IsRotatedProperty); }
            set { SetValue(IsRotatedProperty, value); }
        }
        #endregion

        #region IsPromoted
        public static readonly BindableProperty IsPromotedProperty = BindableProperty.Create(
                                                                            nameof(IsPromoted),
                                                                            typeof(bool),
                                                                            typeof(KomaView),
                                                                            false);
 
        /// <summary>
        /// 反転
        /// </summary>
        public bool IsPromoted
        {
            get { return (bool)GetValue(IsPromotedProperty); }
            set { SetValue(IsPromotedProperty, value); }
        }
        #endregion
    }
}