using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reactive.Bindings;
using MiniShogiMobile.Views;
using Prism.Services;
using MiniShogiMobile.Conditions;

// 参考:<https://anderson02.com/category/cs/xamarin-prism/>

namespace MiniShogiMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ReactiveCommand StartGameCommand { get; set; }
        public ReactiveCommand CreateGameCommand { get; set; }
        public ReactiveCommand ContinueGameCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            StartGameCommand = new ReactiveCommand();
            StartGameCommand.Subscribe(() =>
            {
                navigationService.NavigateAsync(nameof(StartGamePage));
            });

            ContinueGameCommand = new ReactiveCommand();
            ContinueGameCommand.Subscribe(() =>
            {
                navigationService.NavigateAsync(nameof(PlayingGameListPage));
            });
            CreateGameCommand = new ReactiveCommand();
            CreateGameCommand.Subscribe(() =>
            {
                navigationService.NavigateAsync(nameof(CreateGameListPage));
            });
        }
    }
}
