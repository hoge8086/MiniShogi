using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public interface ISelectable
    {
        void Select();
    }
    public interface IViewState
    {
        Task HandleAsync(PlayGamePageViewModel vm, ISelectable cell);
    }
    public class PlayGamePageViewModel : ViewModelBase, GameListener
    {
        private ReactiveProperty<IViewState> ViewState;
        public void ChangeState(IViewState state) => ViewState.Value = state;
        public GameViewModel<CellPlayingViewModel, PlayerWithHandPlayingViewModel, HandKomaPlayingViewModel> Game { get; set; }
        public ReactiveCommand<ISelectable> MoveCommand { get; set; }

        public PlayGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            ViewState = new ReactiveProperty<IViewState>(new ViewStateWaiting());
            Game = new GameViewModel<CellPlayingViewModel, PlayerWithHandPlayingViewModel, HandKomaPlayingViewModel>();
            MoveCommand = new ReactiveCommand<ISelectable>();
            MoveCommand.Subscribe(async x =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    await ViewState.Value.HandleAsync(this, x);
                });
            });
        }

        public async Task AppServiceCallCommandAsync(Action<GameService> action)
        {
            ChangeState(new ViewStateWaiting());
            await Task.Run(() =>
            {
                action(App.GameService);
                if (App.GameService.GetGame().State.IsEnd)
                    ChangeState(new ViewStateGameEnd());
                else
                    ChangeState(new ViewStateHumanThinkingForMoveFrom());
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            await CatchErrorWithMessageAsync(async () =>
            {

                var param = parameters[nameof(PlayGameCondition)] as PlayGameCondition;
                if (param == null)
                    throw new ArgumentException(nameof(PlayGameCondition));

                Title = param.Name;
                App.GameService.Subscribe(this);
                var cancelTokenSource = new CancellationTokenSource();

                await AppServiceCallCommandAsync(service =>
                {
                    service.Start(param.Player1, param.Player2, param.FirstTurnPlayer, param.Name, cancelTokenSource.Token);
                });
            });

        }

        public void UpdateView()
        {
            var game = App.GameService.GetGame();
            Game.Update(game.Board.Height, game.Board.Width, game.State.KomaList);
            Game.HandsOfPlayer1.IsCurrentTurn.Value = game.State.TurnPlayer == PlayerType.Player1;
            Game.HandsOfPlayer2.IsCurrentTurn.Value = game.State.TurnPlayer == PlayerType.Player2;
        }

        public void OnStarted()
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                // [TODO:かっこ悪いので直す]
                // [プレイヤー]
                Game.HandsOfPlayer1.Player.Value = App.GameService.GetPlayingGame().GerPlayer(PlayerType.Player1);
                Game.HandsOfPlayer1.Type.Value = PlayerType.Player1;
                Game.HandsOfPlayer1.TurnType.Value = App.GameService.GetGame().State.TurnPlayer == PlayerType.Player1 ? TurnType.FirstTurn : TurnType.SecondTurn;
                Game.HandsOfPlayer2.Player.Value = App.GameService.GetPlayingGame().GerPlayer(PlayerType.Player2);
                Game.HandsOfPlayer2.Type.Value = PlayerType.Player2;
                Game.HandsOfPlayer2.TurnType.Value = App.GameService.GetGame().State.TurnPlayer == PlayerType.Player2 ? TurnType.FirstTurn : TurnType.SecondTurn;

                UpdateView();
            });
        }

        public void OnPlayed()
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                UpdateView();
            });
        }

        public void OnEnded(PlayerType winner)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = App.GameService.GetPlayingGame().GerPlayer(winner);
                PageDialogService.DisplayAlertAsync(
                     "ゲーム終了",
                     $"勝者: {player.Name}({winner.ToString()})",
                     "OK");
            });
        }
    }

    public class HandKomaPlayingViewModel : HandKomaViewModel, ISelectable
    {
        public ReactiveProperty<bool> IsSelected { get; set; }

        public override void Clear()
        {
            base.Clear();
            IsSelected.Value = false;
        }
        public HandKomaPlayingViewModel()
        {
            IsSelected = new ReactiveProperty<bool>(false);
        }
        public void Select()
        {
            IsSelected.Value = true;
        }
    }

    public class CellPlayingViewModel : CellViewModel, ISelectable
    {
        public ReactiveProperty<List<MoveCommand>> MoveCommands { get; set; }

        public ReadOnlyReactivePropertySlim<bool> CanMove { get; set; }

        public ReactiveProperty<bool> IsSelected { get; set; }
        public CellPlayingViewModel()
        {
            MoveCommands = new ReactiveProperty<List<MoveCommand>>();
            IsSelected = new ReactiveProperty<bool>(false);
            CanMove = MoveCommands.Select((x) => x != null).ToReadOnlyReactivePropertySlim();
        }
        public override void ToEmpty()
        {
            base.ToEmpty();
            IsSelected.Value = false;
            MoveCommands.Value = null;

        }
        public void Select()
        {
            IsSelected.Value = true;
        }
    }
    public enum TurnType
    {
        FirstTurn,
        SecondTurn,
    }

    public class PlayerWithHandPlayingViewModel : HandsViewModel<HandKomaPlayingViewModel>
    {
        public ReactiveProperty<Player> Player { get; set; }
        public ReactiveProperty<PlayerType> Type { get; set; }
        public ReactiveProperty<TurnType> TurnType { get; set; }
        public  ReactiveProperty<bool> IsCurrentTurn { get; }
        public  ReadOnlyReactiveProperty<string> Name { get; }
        public  ReadOnlyReactiveProperty<string> TurnTypeName { get; }

        public string GetName() {
            if (Player.Value == null || Type.Value == null)
                return "";
            return Player.Value.IsAI ? "CPU" : (Type.Value == PlayerType.Player1 ? "P1" : "P2");
        }

        public PlayerWithHandPlayingViewModel()
        {
            Player = new ReactiveProperty<Player>();
            Type = new ReactiveProperty<PlayerType>();
            IsCurrentTurn = new ReactiveProperty<bool>();
            Name = Player.CombineLatest(Type, (x, y) => GetName()).ToReadOnlyReactiveProperty();
            TurnType = new ReactiveProperty<TurnType>();
            TurnTypeName = TurnType.Select(x => x == ViewModels.TurnType.FirstTurn ? "先手" : "後手").ToReadOnlyReactiveProperty();
        }
    }
}
