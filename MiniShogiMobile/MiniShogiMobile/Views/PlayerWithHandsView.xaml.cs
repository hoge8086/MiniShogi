using MiniShogiMobile.Controls;
using Shogi.Business.Domain.Model.Komas;
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
    public partial class PlayerWithHandsView : ContentView
    {
        public PlayerWithHandsView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty ClickCommandProperty =
            BindableProperty.Create(
                nameof(ClickCommand), typeof(ICommand), typeof(PlayerWithHandsView),
                defaultValue: new Command((obj) => {
                    System.Diagnostics.Debug.WriteLine("CellView Tapped");
                }));

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public static readonly BindableProperty KomaLongPressCommandProperty =
            BindableProperty.Create(
                nameof(KomaLongPressCommand), typeof(ICommand), typeof(PlayerWithHandsView),
                defaultValue: new Command((obj) => {
                    System.Diagnostics.Debug.WriteLine("CellView Tapped");
                }));

        public ICommand KomaLongPressCommand
        {
            get { return (ICommand)GetValue(KomaLongPressCommandProperty); }
            set { SetValue(KomaLongPressCommandProperty, value); }
        }
        public static readonly BindableProperty IsRotatedProperty =
            BindableProperty.Create(
                nameof(IsRotated), typeof(bool), typeof(PlayerWithHandsView), false);

        public bool IsRotated
        {
            get { return (bool)GetValue(IsRotatedProperty); }
            set { SetValue(IsRotatedProperty, value); }
        }


        #region PlayerColor
        public static readonly BindableProperty PlayerColorProperty = BindableProperty.Create(
                                                                            nameof(PlayerColor),
                                                                            typeof(Color),
                                                                            typeof(PlayerWithHandsView),
                                                                            null);
 
        /// <summary>
        /// 表示名
        /// </summary>
        public Color PlayerColor
        {
            get { return (Color)GetValue(PlayerColorProperty); }
            set { SetValue(PlayerColorProperty, value); }
        }
        #endregion
        public IEnumerable<HandKomaView> GetHandKomaViews()
        {
            return handsStackLayout.Children.Select(x =>
            {
                var grid = x as Grid;
                return grid.Children[0] as HandKomaView;
            });
        }
    }
}