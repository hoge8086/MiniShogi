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
using Reactive.Bindings.Extensions;

// 参考:<https://anderson02.com/category/cs/xamarin-prism/>

namespace MiniShogiMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public AsyncReactiveCommand StartGameCommand { get; set; }
        public AsyncReactiveCommand CreateGameCommand { get; set; }
        public AsyncReactiveCommand ContinueGameCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            StartGameCommand = new AsyncReactiveCommand();
            StartGameCommand.Subscribe(async () =>
            {
                await navigationService.NavigateAsync(nameof(StartGamePage));
            }).AddTo(Disposable);;

            ContinueGameCommand = new AsyncReactiveCommand();
            ContinueGameCommand.Subscribe(async () =>
            {
                await navigationService .NavigateAsync(nameof(PlayingGameListPage));
            }).AddTo(Disposable);;
            CreateGameCommand = new AsyncReactiveCommand();
            CreateGameCommand.Subscribe(async () =>
            {
                await navigationService.NavigateAsync(nameof(CreateGameListPage));
            }).AddTo(Disposable);;
        }
    }
}
