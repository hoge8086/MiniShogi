using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CellView : ContentView
    {
        public CellView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty ClickCommandProperty =
            BindableProperty.Create(
                nameof(ClickCommand), typeof(ICommand), typeof(CellView),
                defaultValue: new Command((obj) => System.Diagnostics.Debug.WriteLine("CellView Tapped")));

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public static readonly BindableProperty IsMarkedProperty =
            BindableProperty.Create(
                nameof(IsMarked), typeof(bool), typeof(CellView), false);

        public bool IsMarked
        {
            get { return (bool)GetValue(IsMarkedProperty); }
            set { SetValue(IsMarkedProperty, value); }
        }

        public static readonly BindableProperty IsHighlightProperty =
            BindableProperty.Create(
                nameof(IsHighlight), typeof(bool), typeof(CellView), false);

        public bool IsHighlight
        {
            get { return (bool)GetValue(IsHighlightProperty); }
            set { SetValue(IsHighlightProperty, value); }
        }

    }
}