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
            var source = newValue as IEnumerable<object>;

            // 行列が増えた時にセルのサイズを調整するメソッドをアタッチする
            var rows = newValue as INotifyPropertyChanged;
            if(rows != null)
            {
                rows.PropertyChanged += uc.self_SizeChanged;
                foreach(var row in source)
                {
                    var cells = row as INotifyPropertyChanged;
                    if(cells != null)
                        cells.PropertyChanged += uc.self_SizeChanged;
                }
            }

            // デタッチ
            rows = oldValue as INotifyPropertyChanged;
            if(rows != null)
            {
                rows.PropertyChanged -= uc.self_SizeChanged;
                foreach(var row in source)
                {
                    var cells = row as INotifyPropertyChanged;
                    if(cells != null)
                        cells.PropertyChanged -= uc.self_SizeChanged;
                }
            }
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

            // [TODO:プロパティ化 21 = 10(枠線)×2 + 1(下線/右線)]
            var unitX = (self.Width - 21) / cells.Count();
            var unitY = (self.Height - 21) / ItemsSource.Count();

            // [各セルを同じ高さ幅（正方形）にする]
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