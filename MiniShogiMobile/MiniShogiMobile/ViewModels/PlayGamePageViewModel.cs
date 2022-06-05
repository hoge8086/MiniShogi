using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.PlayingGames;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class PlayGamePageViewModel : NavigationViewModel<PlayGameCondition>, GameListener
    {
        public PlayingGame PlayingGame { get; private set; }
        private ReactiveProperty<IViewState> ViewState;
        public void ChangeState(IViewState state) => ViewState.Value = state;
        public GameViewModel<CellPlayingViewModel, PlayerWithHandPlayingViewModel, HandKomaPlayingViewModel> Game { get; set; }
        public AsyncReactiveCommand<ISelectable> MoveCommand { get; private set; }
        public AsyncReactiveCommand SaveCommand { get; private set; }
        public AsyncReactiveCommand StopCommand { get; private set; }
        public AsyncReactiveCommand ResumeCommand { get; private set; }
        public AsyncReactiveCommand UndoCommand { get; private set; }
        public AsyncReactiveCommand RedoCommand { get; private set; }

        private CancellationTokenSource cancelWaiting;

        public Func<MoveCommand, Task> StartAnimationOfKomaMoving;
        public Func<Task> EndAnimationOfKomaMoving;

        public PlayGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            PlayingGame = null;
            ViewState = new ReactiveProperty<IViewState>(new ViewStateWaiting());
            Game = new GameViewModel<CellPlayingViewModel, PlayerWithHandPlayingViewModel, HandKomaPlayingViewModel>();
            MoveCommand = new AsyncReactiveCommand<ISelectable>();
            MoveCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    await ViewState.Value.HandleAsync(this, x);
                });
            }).AddTo(Disposable);

            SaveCommand = ViewState.Select(state => !(state is ViewStateWaiting))
                            .ToAsyncReactiveCommand()
                            .AddTo(this.Disposable);
            SaveCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var gameNameList = App.GameService.PlayingGameRepository.FindAll().Select(y => y.Name).ToList();
                    var result = await NavigateAsync<InputNamePopupPageViewModel, InputNameCondition, string>(
                        new InputNameCondition(
                            "名前を入力してください",
                            $"{DateTime.Now.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("ja-JP"))}_{PlayingGame.GameTemplate.Name}",
                            "保存",
                            gameNameList,
                            null,
                            (n) => gameNameList.Contains(n) ? "既にその名前は使用しています。上書きしますか?" : null));
                    if(result.Success)
                        App.GameService.SaveCurrent(result.Data);
                });

            }).AddTo(Disposable);

            StopCommand = ViewState.Select(state => !(state is ViewStateGameStudying))
                            .ToAsyncReactiveCommand()
                            .AddTo(this.Disposable);
            StopCommand.Subscribe(async x =>
            {
                if (cancelWaiting != null)
                    cancelWaiting?.Cancel();
                else
                    ChangeState(new ViewStateGameStudying());
                
                UpdateView();   // [駒選択中の場合のハイライトをなくすため]

            }).AddTo(Disposable);
            
            ResumeCommand = ViewState.Select(state => state is ViewStateGameStudying)
                            .ToAsyncReactiveCommand()
                            .AddTo(this.Disposable);
            ResumeCommand.Subscribe(async x =>
            {
                await AppServiceCallWithWaitAsync((gameService, cancelToken) =>
                {
                    gameService.Resume(cancelToken);
                });
            }).AddTo(Disposable);

            UndoCommand = ViewState.Select(state => state is ViewStateGameStudying)
                            .ToAsyncReactiveCommand()
                            .AddTo(this.Disposable);
            UndoCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    App.GameService.Undo(Shogi.Business.Domain.Model.Games.Game.UndoType.Undo);
                    PlayingGame = App.GameService.GetCurrentPlayingGame();
                    UpdateView();
                });
            }).AddTo(Disposable);
            RedoCommand = ViewState.Select(state => state is ViewStateGameStudying)
                            .ToAsyncReactiveCommand()
                            .AddTo(this.Disposable);
            RedoCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    App.GameService.Undo(Shogi.Business.Domain.Model.Games.Game.UndoType.Redo);
                    PlayingGame = App.GameService.GetCurrentPlayingGame();
                    UpdateView();
                });
            }).AddTo(Disposable);
        }

        public async Task AppServiceCallWithWaitAsync(Action<GameService, CancellationToken> action)
        {
            try
            {
                cancelWaiting = new CancellationTokenSource();
                ChangeState(new ViewStateWaiting());
                await Task.Run(() =>
                {
                    action(App.GameService, cancelWaiting.Token);
                });
                if (PlayingGame.Game.State.IsEnd)
                    ChangeState(new ViewStateGameStudying());
                else
                    ChangeState(new ViewStateHumanThinkingForMoveFrom());

            }
            catch(OperationCanceledException ex)
            {
                ChangeState(new ViewStateGameStudying());
            }
            cancelWaiting = null;
        }

        public void UpdateView()
        {
            var game = PlayingGame.Game;
            Game.Update(game.Board.Height, game.Board.Width, game.State.KomaList);
            Game.HandsOfPlayer1.IsCurrentTurn.Value = game.State.TurnPlayer == PlayerType.Player1;
            Game.HandsOfPlayer2.IsCurrentTurn.Value = game.State.TurnPlayer == PlayerType.Player2;
        }

        public void OnStarted(PlayingGame playingGame)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                Title = playingGame.GameTemplate.Name;
                PlayingGame = playingGame;
                // [TODO:かっこ悪いので直す]
                // [プレイヤー]
                Game.HandsOfPlayer1.Player.Value = playingGame.GerPlayer(PlayerType.Player1);
                Game.HandsOfPlayer1.TurnType.Value = playingGame.Game.State.TurnPlayer == PlayerType.Player1 ? TurnType.FirstTurn : TurnType.SecondTurn;
                Game.HandsOfPlayer2.Player.Value = playingGame.GerPlayer(PlayerType.Player2);
                Game.HandsOfPlayer2.TurnType.Value = playingGame.Game.State.TurnPlayer == PlayerType.Player2 ? TurnType.FirstTurn : TurnType.SecondTurn;
                UpdateView();
                // MEMO:初手がCPUだと描画が完了する前に手を指してしまい?、
                // 移動用駒が最大化して表示されてしまうため少し待ちを入れる.(CPU側もWait()してるので処理も止まる)
                // 条件: どうぶつ将棋、CPU(4手) vs CPU(4手)、先手:2P
                await Task.Delay(100);
            }).Wait();
        }

        public void OnPlayed(PlayingGame playingGame, MoveCommand moveCommand)
        {
            // Note:MoveCommandを取得して、それによって描画更新を行えば、アニメーションに対応できる
            //      また、最後の着手手が分かるので、最後の着手手をハイライトできる
            Device.InvokeOnMainThreadAsync(async () =>
            {
                PlayingGame = playingGame;
                await StartAnimationOfKomaMoving?.Invoke(moveCommand);
                UpdateView();
                await EndAnimationOfKomaMoving?.Invoke();
            }).Wait();
        }

        public void OnEnded(PlayerType winner)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = PlayingGame.GerPlayer(winner);
                PageDialogService.DisplayAlertAsync(
                     "ゲーム終了",
                     $"勝者: {player.Name}({winner.ToString()})",
                     "OK");
            });
        }
        public void Report(ProgressInfoOfAIThinking value)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = Game.GetHands(value.PlayerType);
                player.ProgressOfComputerThinking.Value = value.ProgressRate;
                //ProgressOfComputerThinking.Value = value.ProgressRate;

                //if(value.ProgressType == ProgressTypeOfAIThinking.Completed)
                //    Evaluation.Value = value.GameEvaluation.Value;
            });
        }
        public override async Task PrepareAsync(PlayGameCondition parameter)
        {
            await this.CatchErrorWithMessageAsync(async () =>
            {

                App.GameService.Subscribe(this);

                if (parameter.PlayMode == PlayMode.NewGame)
                {
                    await AppServiceCallWithWaitAsync((service, cancelToken) =>
                    {
                        service.Start(new List<Player> { parameter.Player1, parameter.Player2 }, parameter.FirstTurnPlayer, parameter.Name, cancelToken);
                    });
                }
                else if(parameter.PlayMode == PlayMode.ContinueGame)
                {
                    await AppServiceCallWithWaitAsync((service, cancelToken) =>
                    {
                        service.Continue(parameter.Name, cancelToken);
                    });
                }
                else
                {
                    throw new ArgumentException(nameof(PlayGameCondition));
                }
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

    public class CellPlayingViewModel : CellViewModel<KomaViewModel>, ISelectable
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
        [Description("先手")]
        FirstTurn,
        [Description("後手")]
        SecondTurn,
    }

    public class PlayerWithHandPlayingViewModel : HandsViewModel<HandKomaPlayingViewModel>
    {
        public ReactiveProperty<Player> Player { get; set; }
        public ReactiveProperty<TurnType> TurnType { get; set; }
        public  ReactiveProperty<bool> IsCurrentTurn { get; }
        public  ReadOnlyReactiveProperty<string> Name { get; }
        public  ReadOnlyReactiveProperty<bool> IsComputer { get; }
        public ReactiveProperty<double> ProgressOfComputerThinking { get; private set; } = new ReactiveProperty<double>();
        public PlayerWithHandPlayingViewModel()
        {
            Player = new ReactiveProperty<Player>();
            IsCurrentTurn = new ReactiveProperty<bool>();
            TurnType = new ReactiveProperty<TurnType>();
            Name = Player.Select(x => x?.Name).ToReadOnlyReactiveProperty();
            IsComputer = Player.Select(x => x == null ? false : x.IsComputer).ToReadOnlyReactiveProperty();
            ProgressOfComputerThinking = new ReactiveProperty<double>();
        }
    }
}

