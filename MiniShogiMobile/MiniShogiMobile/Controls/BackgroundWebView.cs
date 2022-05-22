using MiniShogiMobile.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
// 参考:<https://qiita.com/microwavePC/items/a5af22d68d17e8210868>
namespace MiniShogiMobile.Controls
{
    public class BackgroundWebView : WebView
    {
        public BackgroundWebView() { }

        public string AssetFileName
        {
            set
            {
                this.Source = $"{DependencyService.Get<IBaseUrl>().Get()}{value}";
            }
        }
    }
}
