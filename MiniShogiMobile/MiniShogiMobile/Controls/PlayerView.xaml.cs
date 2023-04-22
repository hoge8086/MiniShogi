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
    public partial class PlayerView : ContentView
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        #region Text
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
                                                                            nameof(Color),
                                                                            typeof(Color),
                                                                            typeof(PlayerView),
                                                                            null);
 
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion
    }
}