using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    /// <summary>
    /// 2Dグリッドコントロール
    /// 自身のコントロ―ル領域で、正方形のセルのグリッドを最大化する(ぴったりでなければ、縦か横のどちらかに余白が残る)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SquareCell2DGridControl : ContentView
    {
        public SquareCell2DGridControl()
        {
            InitializeComponent();
        }
        #region MaxHeight, MaxWidth
        public static readonly BindableProperty MaxHeightProperty = BindableProperty.Create(
                                                                            nameof(MaxHeight),
                                                                            typeof(Double),
                                                                            typeof(SquareCell2DGridControl),
                                                                            -1.0,
                                                                            propertyChanging: OnMaxHeightOrWidthPropertyChanged);
 
        /// <summary>
        /// グリットの各セルのデータテンプレート
        /// </summary>
        public Double MaxHeight
        {
            get { return (Double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        public static readonly BindableProperty MaxWidthProperty = BindableProperty.Create(
                                                                            nameof(MaxWidth),
                                                                            typeof(Double),
                                                                            typeof(SquareCell2DGridControl),
                                                                            -1.0,
                                                                            propertyChanging: OnMaxHeightOrWidthPropertyChanged);
 
        /// <summary>
        /// グリットの各セルのデータテンプレート
        /// </summary>
        public Double MaxWidth
        {
            get { return (Double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        static void OnMaxHeightOrWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = bindable as SquareCell2DGridControl;
            ctrl.board_ChildCountChanged(null, null);
        }
        #endregion

        #region ItemTemplate
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
                                                                            "ItemTemplate",
                                                                            typeof(ControlTemplate),
                                                                            typeof(SquareCell2DGridControl),
                                                                            null,
                                                                            propertyChanging: OnItemTemplatePropertyChanged);
 
        /// <summary>
        /// グリットの各セルのデータテンプレート
        /// </summary>
        public ControlTemplate ItemTemplate
        {
            get { return (ControlTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        static void OnItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = bindable as SquareCell2DGridControl;
            var template = newValue as ControlTemplate;
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
            var newSource = newValue as IEnumerable<object>;
            BindableLayout.SetItemsSource(uc.board, newSource);
        }
        #endregion

        ///// <summary>
        ///// サイズ変更処理
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private void board_ChildCountChanged(object sender, EventArgs e)
        {
            //TODO:バインディング先ではなくStackLayoutのChildrenの数でサイズを求めた方が良い（仮に数に違いがあると問題なので）
            if (ItemsSource == null || ItemsSource.Count() == 0)
                return;

            var cells = ItemsSource.First() as IEnumerable<object>;
            if (cells.Count() == 0)
                return;

            if (MaxHeight <= 0 || MaxWidth <= 0)
                return;


            // [TODO:プロパティ化 21 = 10(枠線)×2 + 1(下線/右線)]
            //var unitX = Math.Floor((Math.Floor(MaxWidth) - (10 + cells.Count() + 1)) / cells.Count());
            //var unitY = Math.Floor((Math.Floor(MaxHeight) - (10 + ItemsSource.Count() + 1)) / ItemsSource.Count());
            //var unitX = Math.Floor(((Math.Floor(MaxWidth) - (20 + (cells.Count() + 1))*2) / cells.Count()));
            //var unitY = Math.Floor(((Math.Floor(MaxHeight) - (20 + (ItemsSource.Count() + 1)*2)) / ItemsSource.Count()));
            var unitX = Math.Floor(((Math.Floor(MaxWidth) - (20 + cells.Count() + 1)) / cells.Count()));
            var unitY = Math.Floor(((Math.Floor(MaxHeight) - (20 + ItemsSource.Count() + 1)) / ItemsSource.Count()));

            // [各セルを同じ高さ幅（正方形）にする]
            var size = Math.Min(unitX, unitY);
            System.Diagnostics.Debug.WriteLine($"cell size:{size}");
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