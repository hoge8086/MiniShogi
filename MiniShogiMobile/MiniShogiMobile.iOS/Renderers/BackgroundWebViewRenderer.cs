using Foundation;
using MiniShogiMobile.Controls;
using MiniShogiMobile.iOS.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BackgroundWebView), typeof(BackgroundWebViewRenderer))]
namespace MiniShogiMobile.iOS.Renderers
{
    public class BackgroundWebViewRenderer : WkWebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // 背景を透過
            this.Opaque = false;
            this.BackgroundColor = UIColor.Clear;
        }
    }
}