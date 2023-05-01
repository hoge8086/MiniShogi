using MiniShogiMobile.Conditions;
using MiniShogiMobile.Utils;
using Prism.Commands;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Application;
using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.AI.Event;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.PlayingGames;
using Shogi.Business.Domain.Model.PlayingGames.Event;
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
    public class PlayGamePageViewModel : NavigationViewModel<PlayGameCondition>
    {
        public PlayingGame PlayingGame { get; private set; }
        public ReactiveProperty<IViewState> ViewState { get; private set; }
        private ReactiveProperty<int> CurrentMoveCount;
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
            CurrentMoveCount = new ReactiveProperty<int>();
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

            UndoCommand = new[]{
                    ViewState.Select(state => state is ViewStateGameStudying),
                    CurrentMoveCount.Select(cnt => (PlayingGame != null) && PlayingGame.Game.CanUndo(Shogi.Business.Domain.Model.Games.Game.UndoType.Undo))
                }.CombineLatestValuesAreAllTrue()
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

            RedoCommand = new[]{
                    ViewState.Select(state => state is ViewStateGameStudying),
                    CurrentMoveCount.Select(cnt => (PlayingGame != null) && PlayingGame.Game.CanUndo(Shogi.Business.Domain.Model.Games.Game.UndoType.Redo))
                }.CombineLatestValuesAreAllTrue()
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
            CurrentMoveCount.Value = game.Record.CurrentMovesCount;
        }

        public void OnGameStarted(GameStarted e)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                Title = e.PlayingGame.GameTemplate.Name;
                PlayingGame = e.PlayingGame;
                // [TODO:かっこ悪いので直す]
                // [プレイヤー]
                Game.HandsOfPlayer1.Player.Value = e.PlayingGame.GerPlayer(PlayerType.Player1);
                Game.HandsOfPlayer1.TurnType.Value = e.PlayingGame.Game.State.TurnPlayer == PlayerType.Player1 ? TurnType.FirstTurn : TurnType.SecondTurn;
                Game.HandsOfPlayer2.Player.Value = e.PlayingGame.GerPlayer(PlayerType.Player2);
                Game.HandsOfPlayer2.TurnType.Value = e.PlayingGame.Game.State.TurnPlayer == PlayerType.Player2 ? TurnType.FirstTurn : TurnType.SecondTurn;
                UpdateView();
                // MEMO:初手がCPUだと描画が完了する前に手を指してしまい?、
                // 移動用駒が最大化して表示されてしまうため少し待ちを入れる.(CPU側もWait()してるので処理も止まる)
                // 条件: どうぶつ将棋、CPU(4手) vs CPU(4手)、先手:2P
                await Task.Delay(100);
            }).Wait();
        }

        public void OnGamePlayed(GamePlayed e)
        {
            // Note:MoveCommandを取得して、それによって描画更新を行えば、アニメーションに対応できる
            //      また、最後の着手手が分かるので、最後の着手手をハイライトできる
            Device.InvokeOnMainThreadAsync(async () =>
            {
                PlayingGame = e.PlayingGame;
                await StartAnimationOfKomaMoving?.Invoke(e.MoveCommand);
                UpdateView();
                await EndAnimationOfKomaMoving?.Invoke();
            }).Wait();
        }

        public void OnGameEnded(GameEnded e)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = PlayingGame.GerPlayer(e.Winner);
                PageDialogService.DisplayAlertAsync(
                     "ゲーム終了",
                     $"勝者: {player.Name}（{EnumToDescriptionConverter.DisplayName(Game.GetHands(e.Winner).TurnType.Value)}）",
                     "OK");
            });
        }
        public void OnComputerThinkingProgressed(ComputerThinkingProgressed e)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = Game.GetHands(e.PlayerType);
                player.ProgressOfComputerThinking.Value = e.ProgressRate.DoubleRate;
            });
        }
        public void OnComputerThinkingStarted(ComputerThinkingStarted e)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = Game.GetHands(e.PlayerType);
                player.ProgressOfComputerThinking.Value = 0.0;
            });
        }
        public void OnComputerThinkingEnded(ComputerThinkingEnded endedEvent)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = Game.GetHands(endedEvent.PlayerType);
                // 次のCPUの進捗表示で一瞬100%が見えるので0%に戻す
                player.ProgressOfComputerThinking.Value = 0.0;

                if(endedEvent.GameEvaluation != null)   //キャンセルの場合はnull
                    player.Evaluation.Value = $"{(int)(endedEvent.GameEvaluation.Value / (double)endedEvent.GameEvaluation.MaxValue * 100)} ({endedEvent.GameEvaluation.Value}/{endedEvent.GameEvaluation.MaxValue})";
            });
        }

        public override async Task PrepareAsync(PlayGameCondition parameter)
        {
            await this.CatchErrorWithMessageAsync(async () =>
            {
                // イベントを購読
                DomainEvents.AddHandler<ComputerThinkingEnded>(OnComputerThinkingEnded);
                DomainEvents.AddHandler<ComputerThinkingProgressed>(OnComputerThinkingProgressed);
                DomainEvents.AddHandler<ComputerThinkingStarted>(OnComputerThinkingStarted);
                DomainEvents.AddHandler<GameEnded>(OnGameEnded);
                DomainEvents.AddHandler<GameStarted>(OnGameStarted);
                DomainEvents.AddHandler<GamePlayed>(OnGamePlayed);

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


        //public async void BackAsync()
        public override async Task<INavigationResult> GoBackAsync(bool? useModalNavigation = null, bool animated = true, Func<Task<bool>> canNavigate = null)
        {
            cancelWaiting?.Cancel();
            // バックグランドの処理が終わるのを待つ
            while (ViewState.Value is ViewStateWaiting)
                await Task.Delay(100);

            bool doSave = await PageDialogService.DisplayAlertAsync("確認", "対局を終了しますか?", "はい", "いいえ");
            if (!doSave)
                // [MEMO:nullを返してしまって問題ないか要確認]
                return await Task.FromResult((INavigationResult)null);

            DomainEvents.RemoveHandler<ComputerThinkingEnded>(OnComputerThinkingEnded);
            DomainEvents.RemoveHandler<ComputerThinkingProgressed>(OnComputerThinkingProgressed);
            DomainEvents.RemoveHandler<ComputerThinkingStarted>(OnComputerThinkingStarted);
            DomainEvents.RemoveHandler<GameEnded>(OnGameEnded);
            DomainEvents.RemoveHandler<GameStarted>(OnGameStarted);
            DomainEvents.RemoveHandler<GamePlayed>(OnGamePlayed);

            return await NavigationService.GoBackToRootAsync();
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
        public ReactiveProperty<string> Evaluation { get; private set; } = new ReactiveProperty<string>("-");
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

