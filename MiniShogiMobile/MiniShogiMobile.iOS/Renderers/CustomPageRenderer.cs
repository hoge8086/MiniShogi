using Foundation;
using MiniShogiMobile.iOS.Renderers;
using MiniShogiMobile.ViewModels;
using Prism.NavigationEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//参考:<https://www.nuits.jp/entry/2016/11/20/185949>
//問題点と処置：トップ画面も戻るボタンが表示されてしまう問題があるため、PLCでトップ画面のナビゲーションバーを無効にする
[assembly: ExportRenderer(typeof(Page), typeof(CustomPageRenderer))]
namespace MiniShogiMobile.iOS.Renderers
{
    public class CustomPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var page = Element as Page;
            var navigationPage = page.Parent as NavigationPage;
            var root = this.NavigationController.TopViewController;
            root.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem($"＜ 戻る", UIBarButtonItemStyle.Done, async (sender, args) =>
            {
                //var navPage = page.Parent as NavigationPage;
                var vm = page.BindingContext as NavigationViewModel;

                if (vm != null)
                {
                    await vm.GoBackAsync();
                    //if (await vm.GoBackAsync())
                    //    navPage.PopAsync();
                }
                //else
                //    navPage.PopAsync();
            }), true);
        }
    }
}