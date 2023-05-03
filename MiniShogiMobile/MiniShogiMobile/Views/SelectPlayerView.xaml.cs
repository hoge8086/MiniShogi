using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectPlayerView : ContentView
    {
        public SelectPlayerView()
        {
            InitializeComponent();
        }

        #region PlayerColor
        public static readonly BindableProperty PlayerColorProperty = BindableProperty.Create(
                                                                            nameof(PlayerColor),
                                                                            typeof(Color),
                                                                            typeof(SelectPlayerView),
                                                                            null);
 
        public Color PlayerColor
        {
            get { return (Color)GetValue(PlayerColorProperty); }
            set { SetValue(PlayerColorProperty, value); }
        }
        #endregion

        #region PlayerTitle
        public static readonly BindableProperty PlayerTitleProperty = BindableProperty.Create(
                                                                            nameof(PlayerTitle),
                                                                            typeof(string),
                                                                            typeof(SelectPlayerView),
                                                                            null);
 
        public string PlayerTitle
        {
            get { return (string)GetValue(PlayerTitleProperty); }
            set { SetValue(PlayerTitleProperty, value); }
        }
        #endregion

        public static readonly BindableProperty MaxThinkingDepthProperty =
            BindableProperty.Create(
                nameof(MaxThinkingDepth), typeof(int), typeof(SelectPlayerView), 5);

        public int MaxThinkingDepth
        {
            get { return (int)GetValue(MaxThinkingDepthProperty); }
            set { SetValue(MaxThinkingDepthProperty, value); }
        }

    }
}