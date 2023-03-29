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

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace MiniShogiMobile.iOS.Renderers
{
    public class BorderlessEntryRenderer: EntryRenderer
    {
        protected override UITextField CreateNativeControl()
        {
            var control = base.CreateNativeControl();
            control.Layer.BorderWidth = 0;
            control.BorderStyle = UITextBorderStyle.None;

            return control;
        }
    }
}