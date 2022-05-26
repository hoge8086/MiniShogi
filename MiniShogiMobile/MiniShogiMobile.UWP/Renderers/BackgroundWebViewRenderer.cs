using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using MiniShogiMobile.Controls;
using MiniShogiMobile.UWP.Renderers;

// 参考:<https://qiita.com/microwavePC/items/a5af22d68d17e8210868>

[assembly: ExportRenderer(typeof(BackgroundWebView), typeof(BackgroundWebViewRenderer))]
namespace MiniShogiMobile.UWP.Renderers
{
    public class BackgroundWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            // 背景を透過
            if(this.Control != null)
            {
                this.Control.DefaultBackgroundColor = Windows.UI.Colors.Transparent;
            }
        }
    }
}
