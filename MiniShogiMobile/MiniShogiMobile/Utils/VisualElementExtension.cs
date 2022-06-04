using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MiniShogiMobile.Utils
{
    public static class VisualElementExtension
    {
        public static Point GetScreenCoords(this VisualElement view, VisualElement root)
        {
            var result = new Point(view.X, view.Y);
            while (view.Parent is VisualElement parent && view.Parent != root)
            {
                result = result.Offset(parent.X, parent.Y);
                view = parent;
            }
            return result;
        }
    }
}
