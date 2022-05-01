using Shogi.Business.Domain.Model.PlayerTypes;
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
    public partial class HandsOfCreateGameView : ContentView
    {
        public HandsOfCreateGameView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TapKomaCommandProperty =
            BindableProperty.Create(
                nameof(TapKomaCommand), typeof(ICommand), typeof(HandsOfCreateGameView),
                defaultValue: new Command((obj) => {
                    System.Diagnostics.Debug.WriteLine("Hand Koma Tapped");
                }));

        public ICommand TapKomaCommand
        {
            get { return (ICommand)GetValue(TapKomaCommandProperty); }
            set { SetValue(TapKomaCommandProperty, value); }
        }

        public static readonly BindableProperty TapKomadaiHighlightCommandProperty =
            BindableProperty.Create(
                nameof(TapKomadaiHighlightCommand), typeof(ICommand), typeof(HandsOfCreateGameView),
                defaultValue: new Command((obj) => {
                    System.Diagnostics.Debug.WriteLine("Komadai Tapped");
                }));

        public ICommand TapKomadaiHighlightCommand
        {
            get { return (ICommand)GetValue(TapKomadaiHighlightCommandProperty); }
            set { SetValue(TapKomadaiHighlightCommandProperty, value); }
        }

        public static readonly BindableProperty AddKomaCommandProperty =
            BindableProperty.Create(
                nameof(AddKomaCommand), typeof(ICommand), typeof(HandsOfCreateGameView),
                defaultValue: new Command((obj) => {
                    System.Diagnostics.Debug.WriteLine("Add Koma Tapped");
                }));

        public ICommand AddKomaCommand
        {
            get { return (ICommand)GetValue(AddKomaCommandProperty); }
            set { SetValue(AddKomaCommandProperty, value); }
        }

        public static readonly BindableProperty PlayerTypeProperty =
            BindableProperty.Create(
                nameof(PlayerType), typeof(PlayerType), typeof(HandsOfCreateGameView), null);

        public PlayerType PlayerType
        {
            get { return (PlayerType)GetValue(PlayerTypeProperty); }
            set { SetValue(PlayerTypeProperty, value); }
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