using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    /// <summary>
    /// 2Dグリッドコントロール
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SquareCell2DGridControl : ContentView
    {
        public SquareCell2DGridControl()
        {
            InitializeComponent();
        }

        #region ItemTemplate
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
                                                                            "ItemTemplate",
                                                                            typeof(DataTemplate),
                                                                            typeof(SquareCell2DGridControl),
                                                                            null,
                                                                            propertyChanging: OnItemTemplatePropertyChanged);
 
        /// <summary>
        /// グリットの各セルのデータテンプレート
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        static void OnItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = bindable as SquareCell2DGridControl;
            var template = newValue as DataTemplate;
            ctrl.ItemTemplate = template;
        }

        #endregion

        #region ItemsSource
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
                                                                                "ItemsSource",
                                                                                typeof(IEnumerable<object>),
                                                                                typeof(SquareCell2DGridControl),
                                                                                null,
                                                                                propertyChanged: OnItemsSourceChanged);
        /// <summary>
        /// セル(ViewModel)の2次元配列
        /// </summary>
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var uc = bindable as SquareCell2DGridControl;
            var source = newValue as IEnumerable<object>;
            uc.ItemsSource = source;
            BindableLayout.SetItemsSource(uc.board, source);
        }
        #endregion

        /// <summary>
        /// サイズ変更処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void self_SizeChanged(object sender, EventArgs e)
        {
            if (ItemsSource == null || ItemsSource.Count() == 0)
                return;

            var cells = ItemsSource.First() as IEnumerable<object>;
            if (cells.Count() == 0)
                return;

            // [各セルを同じ高さ幅（正方形）にする]
            var unitX = self.Width / cells.Count();
            var unitY = self.Height / ItemsSource.Count();

            var size = Math.Min(unitX, unitY);
            foreach(var row in board.Children)
            {
                var stackLayout = row as StackLayout;
                if(stackLayout != null)
                {
                    foreach (var cell in stackLayout.Children)
                    {
                        cell.HeightRequest = size;
                        cell.WidthRequest = size;
                    }
                }
            }
        }
    }
}