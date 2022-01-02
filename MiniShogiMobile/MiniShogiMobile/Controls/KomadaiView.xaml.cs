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
    public partial class KomadaiView : ContentView
    {
        public KomadaiView()
        {
            InitializeComponent();
        }
        #region ItemTemplate
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
                                                                            "ItemTemplate",
                                                                            typeof(DataTemplate),
                                                                            typeof(KomadaiView),
                                                                            null);
 
        /// <summary>
        /// グリットの各セルのデータテンプレート
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        #endregion

        #region ItemsSource
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
                                                                                "ItemsSource",
                                                                                typeof(IEnumerable<object>),
                                                                                typeof(KomadaiView),
                                                                                null);
        /// <summary>
        /// セル(ViewModel)の2次元配列
        /// </summary>
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion
    }
}