using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Prism.Commands;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.GameFactorys;
using System.Linq;
using Shogi.Business.Domain.Model.Komas;
using Prism.Mvvm;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;
using System.Threading.Tasks;
using System.Threading;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace MiniShogiApp.Presentation.ViewModel
{

    // [★条件分岐が多いのでStateパターンか何か使えないか]
    // [★TOOD:基本的にゲーム状態から取ってくるようにするる]
    public enum OperationMode
    {
        SelectMoveSource,       // [人ターン(移動駒選択)]
        SelectMoveDestination,  // [人ターン(移動先選択)]
        AIThinking,             // [AIターン][別スレッドが動作中のためユーザの操作は制限される]
        StopAIThinking,         // [AIターン][別スレッドが動作中のためユーザの操作は制限される]
        GameEnd,
    };

    public class ShogiBoardViewModel : BindableBase, GameListener
    {
        public DelegateCommand<object> MoveCommand { get; set; }
        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand RestartCommand { get; set; }
        public DelegateCommand StopAIThinkingCommand { get; set; }
        public DelegateCommand ResumeAIThinkingCommand { get; set; }
        public ObservableCollection<ObservableCollection<CellViewModel>> Board { get; set; } = new ObservableCollection<ObservableCollection<CellViewModel>>();

        private Player _foregroundPlayer;
        public Player ForegroundPlayer
        {
            get { return _foregroundPlayer; }
            set { SetProperty(ref _foregroundPlayer, value); }
        }
        public PlayerViewModel FirstPlayerHands { get; set; }
        public PlayerViewModel SecondPlayerHands { get; set; }
        private OperationMode _operationMode;
        public OperationMode OperationMode
        {
            get { return _operationMode; }
            set {
                SetProperty(ref _operationMode, value);
                MoveCommand?.RaiseCanExecuteChanged();
                StartCommand?.RaiseCanExecuteChanged();
                RestartCommand?.RaiseCanExecuteChanged();
                StopAIThinkingCommand?.RaiseCanExecuteChanged();
                ResumeAIThinkingCommand?.RaiseCanExecuteChanged();
            }

        }
        private CancellationTokenSource cancelTokenSource;

        private GameService gameService;

        private IMessenger Messenger;
        public ShogiBoardViewModel(GameService gameService, IMessenger messenger, Func<GameSet> gameSetGetter)
        {
            Messenger = messenger;
            this.gameService = gameService;

            OperationMode = OperationMode.SelectMoveSource;
            this.gameService.Subscribe(this);

            StartCommand = new DelegateCommand(
                async () =>
                {
                    var gameSet = gameSetGetter();
                    FirstPlayerHands.Name = gameSet.Players[PlayerType.FirstPlayer].Name;
                    SecondPlayerHands.Name = gameSet.Players[PlayerType.SecondPlayer].Name;
                    OperationMode = OperationMode.AIThinking;
                    cancelTokenSource = new CancellationTokenSource();
                    await Task.Run(() =>
                    {
                        this.gameService.Start(gameSet, cancelTokenSource.Token);
                        //this.gameService.Start(new NegaAlphaAI(9), new NegaAlphaAI(9), GameType.AnimalShogi, this);
                    });
                    cancelTokenSource = null;
                    // [MEMO:タスクが完了されるまでここは実行されない(AIThinkingのまま)]
                    UpdateOperationModeOnTaskFinished();
                },
                () =>
                {
                    return OperationMode != OperationMode.AIThinking;
                });
            RestartCommand = new DelegateCommand(
                async () =>
                {
                    OperationMode = OperationMode.AIThinking;
                    cancelTokenSource = new CancellationTokenSource();
                    await Task.Run(() =>
                    {
                        this.gameService.Restart(cancelTokenSource.Token);
                    });
                    cancelTokenSource = null;
                    // [MEMO:タスクが完了されるまでここは実行されない(AIThinkingのまま)]
                    UpdateOperationModeOnTaskFinished();

                },
                () =>
                {
                    return OperationMode != OperationMode.AIThinking;
                });

            MoveCommand = new DelegateCommand<object>(
                async (param) =>
                {
                    if (param == null)
                        return;

                    if (OperationMode == OperationMode.SelectMoveSource)
                    {
                        var selectedMoveSource = param as ISelectable;
                        selectedMoveSource.IsSelected = true;
                        OperationMode = OperationMode.SelectMoveDestination;

                        UpdateCanMove(selectedMoveSource);
                    }
                    else if(OperationMode == OperationMode.SelectMoveDestination)
                    {

                        var cell = param as CellViewModel;

                        if(cell != null && cell.CanMove)
                        {
                            MoveCommand move = null;
                            if(cell.MoveCommands.Count() == 1)
                            {
                                move = cell.MoveCommands[0];
                            }
                            else
                            {
                                var doTransform = Messenger.MessageYesNo("成りますか?");
                                move = cell.MoveCommands.FirstOrDefault(x => x.DoTransform == doTransform);
                            }
                            //game.Play(move);
                            OperationMode = OperationMode.AIThinking;
                            cancelTokenSource = new CancellationTokenSource();
                            await Task.Run(() => this.gameService.Play(move, cancelTokenSource.Token));
                            cancelTokenSource = null;
                            // [MEMO:タスクが完了されるまでここは実行されない(AIThinkingのまま)]
                            UpdateOperationModeOnTaskFinished();
                        }
                        else
                        {
                            // [動けない位置の場合はキャンセルしすべて更新]
                            OperationMode = OperationMode.SelectMoveSource;
                            Update();
                        }
                    }
                },
                (param) =>
                {
                    if (param == null)
                        return false;

                    if (OperationMode == OperationMode.GameEnd)
                        return false;

                    if (OperationMode == OperationMode.SelectMoveSource)
                    {
                        var cell = param as CellViewModel;
                        if (cell != null)
                        {
                            if (cell.Koma == null)
                                return false;

                            return this.gameService.GetGame().State.TurnPlayer == cell.Koma.Player.ToDomain();
                        }

                        var hand = param as HandKomaViewModel;
                        if(hand != null)
                        {
                            return this.gameService.GetGame().State.TurnPlayer == hand.Player.ToDomain();
                        }
                    }
                    else if (OperationMode == OperationMode.SelectMoveDestination)
                    {
                        return true;
                    }
                    
                    // [OperationMode.AIThinking]
                    return false;
                }
                );

            StopAIThinkingCommand = new DelegateCommand(
                () =>
                {
                    cancelTokenSource?.Cancel();
                },
                () =>
                {
                    return OperationMode == OperationMode.AIThinking;
                });
            ResumeAIThinkingCommand = new DelegateCommand(
                async () =>
                {
                    OperationMode = OperationMode.AIThinking;
                    cancelTokenSource = new CancellationTokenSource();
                    await Task.Run(() =>
                    {
                        this.gameService.Resume(cancelTokenSource.Token);
                    });
                    cancelTokenSource = null;
                    // [MEMO:タスクが完了されるまでここは実行されない(AIThinkingのまま)]
                    UpdateOperationModeOnTaskFinished();

                },
                () =>
                {
                    return OperationMode == OperationMode.StopAIThinking;
                });
            var ai = new NegaAlphaAI(6);
            var human = new Human();
            FirstPlayerHands = new PlayerViewModel() { Player = Player.FirstPlayer , Name=human.Name};
            SecondPlayerHands = new PlayerViewModel() { Player = Player.SecondPlayer , Name= ai.Name};
            ForegroundPlayer = Player.FirstPlayer;

            // [MEMO:タスクで開始していない(コンストラクタなのできない)ので、必ず初手はHumanになるようにする]
            cancelTokenSource = new CancellationTokenSource();
            this.gameService.Start(new GameSet(human, ai, GameType.AnimalShogi), cancelTokenSource.Token);
            cancelTokenSource = null;
        }
        public void UpdateOperationModeOnTaskFinished()
        {
            // [タスクが終わったというこは、自分のターンかゲーム終了かのどちらか]
            if(gameService.GetGame().IsEnd)
            {
                OperationMode = OperationMode.GameEnd;
                return;
            }
            if(gameService.IsTurnPlayerAI())
            {
                // [AI思考停止中]
                OperationMode = OperationMode.StopAIThinking;
            }
            else
            {
                // [人の駒選択]
                OperationMode = OperationMode.SelectMoveSource;
            }
        }

        public void Update()
        {
            Board.Clear();
            FirstPlayerHands.Hands.Clear();
            SecondPlayerHands.Hands.Clear();

            for (int y = 0; y < this.gameService.GetGame().Board.Height; y++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (int x = 0; x < this.gameService.GetGame().Board.Width; x++)
                    row.Add(new CellViewModel() { Koma = null, Position = new BoardPosition(x, y), MoveCommands = null});
                Board.Add(row);
            }

            foreach (var koma in this.gameService.GetGame().State.KomaList)
            {
                if (koma.BoardPosition != null)
                {
                    var cell = Board[koma.BoardPosition.Y][koma.BoardPosition.X];
                    cell.Koma = new KomaViewModel()
                    {
                        IsTransformed = koma.IsTransformed,
                        Name = koma.KomaType.Id,
                        Player = koma.Player == PlayerType.FirstPlayer ? Player.FirstPlayer : Player.SecondPlayer,
                    };
                }
                else
                {
                    if (koma.Player == PlayerType.FirstPlayer)
                        FirstPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.FirstPlayer});
                    else
                        SecondPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.SecondPlayer });
                }
            }
            
            FirstPlayerHands.IsCurrentTurn = this.gameService.GetGame().State.TurnPlayer == PlayerType.FirstPlayer;
            SecondPlayerHands.IsCurrentTurn = this.gameService.GetGame().State.TurnPlayer == PlayerType.SecondPlayer;

            MoveCommand.RaiseCanExecuteChanged();
        }
        public void UpdateCanMove(ISelectable selectedMoveSource)
        {
            // [MEMO:ここでUpdate()(つまり、Board.Clear())してしまうと、持ち駒で同じ種類がある場合にどれをハイライトすべきか判別できなくなる]
            if (OperationMode != OperationMode.SelectMoveDestination)
                return;

            Koma koma = selectedMoveSource.GetKoma(this.gameService.GetGame());

            var moves = this.gameService.GetGame().CreateAvailableMoveCommand(koma);
            foreach(var row in Board)
            {
                foreach(var cell in row)
                {
                    var cellMoves = moves.Where(x => x.ToPosition == cell.Position).ToList();
                    if(cellMoves.Count() > 0)
                        cell.MoveCommands = cellMoves;
                }
            }

            MoveCommand.RaiseCanExecuteChanged();
        }
        public void OnStarted()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Update();
            });
        }
        public void OnPlayed()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Update();
            });
        }
        public void OnEnded(PlayerType winner)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var winnerName = winner == PlayerType.FirstPlayer ? FirstPlayerHands.Name : SecondPlayerHands.Name;
                Messenger.Message("勝者: " + winnerName + "(" + winner.ToString() + ")");
                MoveCommand.RaiseCanExecuteChanged();
            });
        }
    }
}
