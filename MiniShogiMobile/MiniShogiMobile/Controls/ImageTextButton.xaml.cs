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
    public partial class ImageTextButton : ContentView
    {
        public ImageTextButton()
        {
            InitializeComponent();
        }
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command), typeof(ICommand), typeof(ImageTextButton),
                defaultValue: new Command((obj) => System.Diagnostics.Debug.WriteLine("CellView Tapped")),
                propertyChanged: OnCommandChanged);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        static void OnCommandChanged (BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as ImageTextButton;
            if (view == null || newValue == oldValue)
                return;

            var newCommand = (ICommand)newValue;
            var oldCommand = (ICommand)oldValue;

            if(newCommand != null)
            {
                newCommand.CanExecuteChanged += view.OnChangeCanExecute;
            }
            if(oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= view.OnChangeCanExecute;
            }

            view.OnChangeCanExecute(null, null);
        }

        private void OnChangeCanExecute(object sender, EventArgs e)
        {
            if(this.Command == null || !this.Command.CanExecute(null))
                this.Opacity = 0.4;
            else
                this.Opacity = 1;
        }

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source), typeof(ImageSource), typeof(ImageTextButton),
                defaultValue: default(ImageSource));

        public ImageSource  Source
        {
            get { return (ImageSource )GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }


        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text), typeof(string), typeof(ImageTextButton),
                defaultValue: string.Empty);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

    }
}