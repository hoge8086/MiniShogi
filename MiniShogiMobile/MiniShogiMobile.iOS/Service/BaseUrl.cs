using Foundation;
using MiniShogiMobile.iOS.Service;
using MiniShogiMobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace MiniShogiMobile.iOS.Service
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundleUrl.ToString();
        }
    }
}