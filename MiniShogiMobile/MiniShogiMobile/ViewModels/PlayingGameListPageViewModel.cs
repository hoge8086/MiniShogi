using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.PlayingGames;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class PlayingGameListPageViewModel : ViewModelBase
    {
        public ReactiveProperty<PlayingGame> SelectedPlayingGame { get; }
        public ObservableCollection<PlayingGame> PlayingGameList { get; }
        public ReactiveCommand PlayCommand { get; set; }
        public ReactiveCommand<PlayingGame> DeleteCommand {get;}

        public PlayingGameListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            SelectedPlayingGame = new ReactiveProperty<PlayingGame>();
            PlayingGameList = new ObservableCollection<PlayingGame>(App.GameService.PlayingGameRepository.FindAll());
            PlayCommand = new ReactiveCommand();
            PlayCommand.Subscribe(() =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(PlayGameCondition), new PlayGameCondition(PlayMode.ContinueGame, SelectedPlayingGame.Value.Name));
                navigationService.NavigateAsync(nameof(PlayGamePage), param);
            }).AddTo(Disposable);

            DeleteCommand = new ReactiveCommand<PlayingGame>();
            DeleteCommand.Subscribe(async (x) =>
            {
                bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                if (doDelete && x != null)
                {
                    App.GameService.PlayingGameRepository.RemoveByName(x.Name);
                    PlayingGameList.Remove(x);
                }
            }).AddTo(Disposable);
        }
    }
}
