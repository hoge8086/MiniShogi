using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reactive.Bindings;
using MiniShogiMobile.Views;

// 参考:<https://anderson02.com/category/cs/xamarin-prism/>

namespace MiniShogiMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ReactiveCommand StartGameCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            StartGameCommand = new ReactiveCommand();
            StartGameCommand.Subscribe(() =>
            {
                navigationService.NavigateAsync(nameof(StartGamePage));
            });
        }
    }
}
