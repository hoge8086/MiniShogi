using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SwitchablePicker : ContentView
    {
        public SwitchablePicker()
        {
            InitializeComponent();
        }

        void OnItemTapped(object sender, EventArgs args)
        {
            if (IsReadOnly)
                return;

            var tappedGrid = sender as Grid;
            if (tappedGrid == null)
                return;

            SelectedItem = tappedGrid.BindingContext;
        }
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(SwitchablePicker),
            null,
            BindingMode.TwoWay);

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(SwitchablePicker));

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// 注意：BindablePropertyではないため、ItemsSourceプロパティより先にこちらをxaml上で指定する必要あり
        ///       ItemDisplayBindingPropertyの実装は参考がなく難しかったので諦め
        /// </summary>
        public IValueConverter DisplayConverter { get; set; }

        private void StackLayout_ChildAdded(object sender, ElementEventArgs e)
        {
            var label = ((Grid)e.Element).Children.FirstOrDefault(x => x is Label) as Label;
            if(DisplayConverter != null)
            {
                var binding = new Binding();
                binding.Converter = DisplayConverter;
                label.SetBinding(Label.TextProperty, binding);
            }
        }

        #region IsReadOnly
        public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
                                                                            nameof(IsReadOnly),
                                                                            typeof(bool),
                                                                            typeof(SwitchablePicker),
                                                                            false);
 
        /// <summary>
        /// 読み取り専用かどうか
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        #endregion
    }
}