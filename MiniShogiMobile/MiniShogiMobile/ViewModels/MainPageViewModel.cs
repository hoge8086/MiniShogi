using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reactive.Bindings;
using MiniShogiMobile.Views;
using MiniShogiMobile.Conditions;

// 参考:<https://anderson02.com/category/cs/xamarin-prism/>

namespace MiniShogiMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ReactiveCommand MovePage { get; set; }
        public ReactiveProperty<string> Label { get; set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Label = new ReactiveProperty<string>("あいうえお");
            MovePage = new ReactiveCommand();
            MovePage.Subscribe(() =>
            {
                var param = new NavigationParameters
                {
                    {nameof(PlayGameCondition), new PlayGameCondition("aaa")}
                };
                navigationService.NavigateAsync(nameof(PlayGamePage), param);
            });
            Title = "Main Page";
        }
    }
}
