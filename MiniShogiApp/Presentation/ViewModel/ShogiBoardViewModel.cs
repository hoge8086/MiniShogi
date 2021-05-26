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
using Shogi.Business.Domain.Model.Users;
using Shogi.Business.Domain.Model.AI;
using System.Threading.Tasks;

namespace MiniShogiApp.Presentation.ViewModel
{

    // [★条件分岐が多いのでStateパターンか何か使えないか]
    public enum OperationMode
    {
        SelectMoveSource,
        SelectMoveDestination,
        AIThinking,
        GameEnd,
    };

    public class ShogiBoardViewModel : BindableBase, GameListener
    {
        public DelegateCommand<object> MoveCommand { get; set; }
        public DelegateCommand StartCommand { get; set; }
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
            }
        }

        private GameService gameService;

        private IMessenger Messenger;
        public ShogiBoardViewModel(GameService gameService, IMessenger messenger)
        {
            Messenger = messenger;
            this.gameService = gameService;

            OperationMode = OperationMode.SelectMoveSource;

            StartCommand = new DelegateCommand(
                async () =>
                {
                    OperationMode = OperationMode.AIThinking;
                    await Task.Run(() =>
                    {
                        //this.gameService.Start(new Human(), new NegaAlphaAI(7), GameType.AnimalShogi, this);
                        //this.gameService.Start(new Human(), new RandomAI(), GameType.AnimalShogi, this);
                        //this.gameService.Start(new Human(), new NegaAlphaAI(5), GameType.FiveFiveShogi, this);
                        //this.gameService.Start(new RandomAI(), new RandomAI(), GameType.FiveFiveShogi, this);
                        //this.gameService.Start(new Human(), new Human(), GameType.FiveFiveShogi, this);
                        this.gameService.Start(new Human(), new Human(), GameType.AnimalShogi, this);
                    });
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
                            await Task.Run(() => this.gameService.Play(move));
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
            FirstPlayerHands = new PlayerViewModel() { Player = Player.FirstPlayer };
            SecondPlayerHands = new PlayerViewModel() { Player = Player.SecondPlayer };
            ForegroundPlayer = Player.FirstPlayer;

            //this.gameService.Start(new Human(), new RandomAI(), GameType.FiveFiveShogi, this);
            //Update();
        }
        public void UpdateOperationModeOnTaskFinished()
        {
            // [タスクが終わったというこは、自分のターンかゲーム終了かのどちらか]
            if(gameService.GetGame().IsEnd)
                OperationMode = OperationMode.GameEnd;
            OperationMode = OperationMode.SelectMoveSource;
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
                        Player = koma.Player == Shogi.Business.Domain.Model.Players.Player.FirstPlayer ? Player.FirstPlayer : Player.SecondPlayer,
                    };
                }
                else
                {
                    if (koma.Player == Shogi.Business.Domain.Model.Players.Player.FirstPlayer)
                        FirstPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.FirstPlayer});
                    else
                        SecondPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.SecondPlayer });
                }
            }
            
            FirstPlayerHands.IsCurrentTurn = this.gameService.GetGame().State.TurnPlayer == Shogi.Business.Domain.Model.Players.Player.FirstPlayer;
            SecondPlayerHands.IsCurrentTurn = this.gameService.GetGame().State.TurnPlayer == Shogi.Business.Domain.Model.Players.Player.SecondPlayer;


            //if(this.gameService.GetGame().IsEnd)
            //{
            //    OperationMode = OperationMode.GameEnd;
            //    Messenger.Message("勝者: " + this.gameService.GetGame().GameResult.Winner.ToString());
            //}

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

        public void OnEnded()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Messenger.Message("勝者: " + this.gameService.GetGame().GameResult.Winner.ToString());
                MoveCommand.RaiseCanExecuteChanged();
            });
        }

    }
}
