using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MiniShogiMobile.Controls
{
    /// <summary>
    /// 選択中の画像のみを表示するコントロール
    /// (ImageクラスでSouceを切り替えるとgifのロードが遅いので、事前にImageに画像を全てロードして表示を切り替える)
    /// TODO:ImageのIsLoadingプロパティを全てまとめたプロパティを公開して、それがTrueになるまで待たせる
    /// </summary>
    public class ImageSelector : Grid
    {
        private Dictionary<string, Image> Images = new Dictionary<string, Image>();

        #region ItemsSource
        public static readonly BindableProperty ImageSourcesProperty = BindableProperty.Create(
                                                                                nameof(ImageSources),
                                                                                typeof(IEnumerable<string>),
                                                                                typeof(ImageSelector),
                                                                                null,
                                                                                propertyChanged: OnImagesChanged);
        public IEnumerable<string> ImageSources
        {
            get { return (IEnumerable<string>)GetValue(ImageSourcesProperty); }
            set { SetValue(ImageSourcesProperty, value); }
        }

        static void OnImagesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var grid = bindable as ImageSelector;
            var newSource = newValue as IEnumerable<string>;
            grid.Children.Clear();
            grid.Images.Clear();
            foreach(var path in newSource)
            {
                var image = new Image() {
                                    Source = path,
                                    IsVisible = path == grid.SelectedImage,
                                    IsAnimationPlaying = (path == grid.SelectedImage) && grid.IsAnimationPlaying
                };
                grid.Children.Add(image);
                grid.Images.Add(path, image);
            }
        }
        #endregion

        public static readonly BindableProperty SelectedImageProperty = BindableProperty.Create(
                                                                            nameof(SelectedImage),
                                                                            typeof(string),
                                                                            typeof(ImageSelector),
                                                                            null,
                                                                            BindingMode.OneWay,
                                                                            propertyChanged: OnSelectedImageChanged);
 
        public string SelectedImage
        {
            get { return (string)GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }
        static void OnSelectedImageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var grid = bindable as ImageSelector;
            var selected = newValue as string;
            foreach(var image in grid.Images)
            {
                image.Value.IsVisible = image.Key == selected;
                image.Value.IsAnimationPlaying = (image.Key == selected) && grid.IsAnimationPlaying;
            }
        }

        public static readonly BindableProperty IsAnimationPlayingProperty = BindableProperty.Create(
                                                                            nameof(IsAnimationPlaying),
                                                                            typeof(bool),
                                                                            typeof(ImageSelector),
                                                                            false,
                                                                            BindingMode.OneWay,
                                                                            propertyChanged: OnIsAnimationPlayingChanged);
 
        public bool IsAnimationPlaying 
        {
            get { return (bool)GetValue(IsAnimationPlayingProperty); }
            set { SetValue(IsAnimationPlayingProperty, value); }
        }
        static void OnIsAnimationPlayingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var grid = bindable as ImageSelector;
            if(grid.CurrentImage != null)
               grid.CurrentImage.IsAnimationPlaying = grid.IsAnimationPlaying;
        }

        private Image CurrentImage
        {
            get {
                if (!Images.ContainsKey(SelectedImage))
                    return null;
                return Images[SelectedImage];
            }
        }

    }
}
