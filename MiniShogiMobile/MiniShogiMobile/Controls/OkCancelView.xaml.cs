using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OkCancelView : ContentView
    {
        public OkCancelView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty OkCommandProperty =
            BindableProperty.Create(
                nameof(OkCommand), typeof(ICommand), typeof(OkCancelView),
                defaultValue: new Command((obj) => System.Diagnostics.Debug.WriteLine("CellView Tapped")));

        public ICommand OkCommand
        {
            get { return (ICommand)GetValue(OkCommandProperty); }
            set { SetValue(OkCommandProperty, value); }
        }

        public static readonly BindableProperty CancelCommandProperty =
            BindableProperty.Create(
                nameof(CancelCommand), typeof(ICommand), typeof(OkCancelView),
                defaultValue: new Command((obj) => System.Diagnostics.Debug.WriteLine("CellView Tapped")));

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }
    }
}