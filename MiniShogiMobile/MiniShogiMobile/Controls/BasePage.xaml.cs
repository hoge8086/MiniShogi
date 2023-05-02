using MiniShogiMobile.Service;
using MiniShogiMobile.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasePage : ContentPage
    {
        public BasePage()
        {
            InitializeComponent();
        }
        
        // MEMO:BasePageに対応するViewModelが作れないので(NavigationViewModelを継承しているため)、
        // 仕方なしにコードビハインドで広告ユニットIDを指定
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var adView = this.GetTemplateChild("adView") as MarcTron.Plugin.Controls.MTAdView;
            if(adView != null ) 
                adView.AdsId = DependencyService.Get<ISettingService>().PrivateSetting.AdUnitIdForBanner;

        }
    }
}