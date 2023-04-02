using Foundation;
using MiniShogiMobile.iOS.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//参考:<https://www.nuits.jp/entry/2016/11/20/185949>
[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace MiniShogiMobile.iOS.Renderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        public override void SetViewControllers(UIViewController[] controllers, bool animated)
        {
            base.SetViewControllers(controllers, animated);
            foreach (var controller in controllers)
            {
                // Disable swipe back
                ((UINavigationController)controller).InteractivePopGestureRecognizer.Enabled = false;
            }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (InteractivePopGestureRecognizer != null)
            {
                InteractivePopGestureRecognizer.Enabled = false;
            }
        }
    }
}