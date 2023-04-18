using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.PlayingGames;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class PlayingGameListPageViewModel : NavigationViewModel
    {
        public ReactiveProperty<PlayingGame> SelectedPlayingGame { get; }
        public ObservableCollection<PlayingGame> PlayingGameList { get; }
        public AsyncReactiveCommand PlayCommand { get; set; }
        public AsyncReactiveCommand DeleteCommand {get;}

        public PlayingGameListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            SelectedPlayingGame = new ReactiveProperty<PlayingGame>();
            PlayingGameList = new ObservableCollection<PlayingGame>(App.GameService.PlayingGameRepository.FindAll());
            PlayCommand = SelectedPlayingGame.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            PlayCommand.Subscribe(async () =>
            {
                await NavigateAsync<PlayGamePageViewModel, PlayGameCondition>(new PlayGameCondition(PlayMode.ContinueGame, SelectedPlayingGame.Value.Name));
            }).AddTo(Disposable);

            DeleteCommand = SelectedPlayingGame.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteCommand.Subscribe(async () =>
            {
                bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                if (doDelete && SelectedPlayingGame.Value != null)
                {
                    App.GameService.PlayingGameRepository.RemoveByName(SelectedPlayingGame.Value.Name);
                    PlayingGameList.Remove(SelectedPlayingGame.Value);
                    SelectedPlayingGame.Value = null;
                }
            }).AddTo(Disposable);
        }
    }
}
