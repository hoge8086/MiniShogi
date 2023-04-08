using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MiniShogiMobile.Controls
{
    public class TintImage : CachedImage
    {
        TintTransformation _Tint;

        public TintImage()
        {
            _Tint = new TintTransformation {
                EnableSolidColor = true
            };
        }

        private void ApplyTransformations()
        {
            Transformations = new List<ITransformation> {
                _Tint
            };
        }

        protected void OnTintColorChanged()
        {
            _Tint.R = (int)(TintColor.R * 255);
            _Tint.G = (int)(TintColor.G * 255);
            _Tint.B = (int)(TintColor.B * 255);
            _Tint.A = (int)(TintColor.A * 255);

            ApplyTransformations();
        }

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            "TintColor", typeof(Color), typeof(TintImage), Color.Default,
            propertyChanged: (bindable, oldValue, newValue) => ((TintImage)bindable).OnTintColorChanged()
        );

        public Color TintColor {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }
    }
}
