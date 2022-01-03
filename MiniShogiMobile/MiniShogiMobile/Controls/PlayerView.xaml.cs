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
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                                            nameof(Text),
                                                                            typeof(string ),
                                                                            typeof(PlayerView),
                                                                            null);
 
        /// <summary>
        /// 表示名
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region SubText
        public static readonly BindableProperty SubTextProperty = BindableProperty.Create(
                                                                            nameof(SubText),
                                                                            typeof(string ),
                                                                            typeof(PlayerView),
                                                                            null);
 
        /// <summary>
        /// 表示名
        /// </summary>
        public string SubText
        {
            get { return (string)GetValue(SubTextProperty); }
            set { SetValue(SubTextProperty, value); }
        }
        #endregion
    }
}