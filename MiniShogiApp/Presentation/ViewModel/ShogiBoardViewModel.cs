using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Prism.Commands;
using Shogi.Bussiness.Domain.Model.Games;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Business.Domain.GameFactory;
using System.Linq;
using Shogi.Bussiness.Domain.Model.Komas;
using Prism.Mvvm;

namespace MiniShogiApp.Presentation.ViewModel
{

    // [★条件分岐が多いのでStateパターンか何か使えないか]
    public enum OperationMode
    {
        SelectMoveSource,
        SelectMoveDestination,
        GameEnd,
    };

    public class ShogiBoardViewModel : BindableBase
    {
        public DelegateCommand<object> MoveCommand { get; set; }
        public ObservableCollection<ObservableCollection<CellViewModel>> Board { get; set; } = new ObservableCollection<ObservableCollection<CellViewModel>>();

        private Player _foregroundPlayer;
        public Player ForegroundPlayer
        {
            get { return _foregroundPlayer; }
            set { SetProperty(ref _foregroundPlayer, value); }
        }
        public PlayerViewModel FirstPlayerHands { get; set; }
        public PlayerViewModel SecondPlayerHands { get; set; }
        public OperationMode OperationMode { get; private set; }

        private Game game;

        private IMessage Message;
        public ShogiBoardViewModel(IMessage message)
        {
            Message = message;
            //game = new GameFactory().Create(GameType.AnimalShogi);
            game = new GameFactory().Create(GameType.FiveFiveShogi);
            OperationMode = OperationMode.SelectMoveSource;

            MoveCommand = new DelegateCommand<object>(
                (param) =>
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
                                var doTransform = Message.MessageYesNo("成りますか?");
                                move = cell.MoveCommands.FirstOrDefault(x => x.DoTransform == doTransform);
                            }
                            game.Play(move);
                        }

                        // [動けない位置の場合はキャンセルしすべて更新]
                        OperationMode = OperationMode.SelectMoveSource;
                        Update();
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

                            return game.State.TurnPlayer == cell.Koma.Player.ToDomain();
                        }

                        var hand = param as HandKomaViewModel;
                        if(hand != null)
                        {
                            return game.State.TurnPlayer == hand.Player.ToDomain();
                        }
                    }
                    else if (OperationMode == OperationMode.SelectMoveDestination)
                    {
                        return true;
                    }

                    return false;
                }
                );
            FirstPlayerHands = new PlayerViewModel() { Player = Player.FirstPlayer };
            SecondPlayerHands = new PlayerViewModel() { Player = Player.SecondPlayer };
            ForegroundPlayer = Player.FirstPlayer;
            Update();
        }

        public void Update()
        {
            Board.Clear();
            FirstPlayerHands.Hands.Clear();
            SecondPlayerHands.Hands.Clear();

            for (int y = 0; y < game.Board.Height; y++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (int x = 0; x < game.Board.Width; x++)
                    row.Add(new CellViewModel() { Koma = null, Position = new BoardPosition(x, y), MoveCommands = null});
                Board.Add(row);
            }

            foreach (var koma in game.State.KomaList)
            {
                if (koma.BoardPosition != null)
                {
                    var cell = Board[koma.BoardPosition.Y][koma.BoardPosition.X];
                    cell.Koma = new KomaViewModel()
                    {
                        IsTransformed = koma.IsTransformed,
                        Name = koma.KomaType.Id,
                        Player = koma.Player == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer ? Player.FirstPlayer : Player.SecondPlayer,
                    };
                }
                else
                {
                    if (koma.Player == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer)
                        FirstPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.FirstPlayer});
                    else
                        SecondPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.SecondPlayer });
                }
            }

            // [★このコードはゴミすぎるので直す]
            if (game.State.TurnPlayer == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer)
            {
                FirstPlayerHands.IsCurrentTurn = true;
                SecondPlayerHands.IsCurrentTurn = false;
            }
            else
            {
                FirstPlayerHands.IsCurrentTurn = false;
                SecondPlayerHands.IsCurrentTurn = true;
            }


            if(game.IsEnd)
            {
                OperationMode = OperationMode.GameEnd;
                Message.Message("勝者: " + game.GameResult.Winner.ToString());
            }

            MoveCommand.RaiseCanExecuteChanged();
        }
        public void UpdateCanMove(ISelectable selectedMoveSource)
        {
            // [MEMO:ここでUpdate()(つまり、Board.Clear())してしまうと、持ち駒で同じ種類がある場合にどれをハイライトすべきか判別できなくなる]
            if (OperationMode != OperationMode.SelectMoveDestination)
                return;

            var selectedCell = selectedMoveSource as CellViewModel;
            var selectedHand = selectedMoveSource as HandKomaViewModel;

            // [★少し気持ち悪い、任意の駒(盤上/手持ち)を示すためのオブジェクトを用意したほうが良い?]
            Koma koma = null;
            if(selectedCell != null)
                koma = game.State.FindBoardKoma(selectedCell.Position);
            if(selectedHand != null)
                koma = game.State.FindHandKoma(selectedHand.Player.ToDomain(), selectedHand.KomaType);

            var moves = game.CreateAvailableMoveCommand(koma);
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
    }
}
