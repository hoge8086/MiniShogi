﻿using System;
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

    public enum OperationMode
    {
        SelectMoveFrom,
        SelectMoveTo,
        Wait,
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
        public PlayerViewModel FirstPlayer { get; set; }
        public PlayerViewModel SecondPlayer { get; set; }
        public OperationMode OperationMode { get; private set; }
        // [★後で直す]
        private ISelectable selectedMoveFrom;

        private Game game;
        public ShogiBoardViewModel()
        {
            ForegroundPlayer = Player.FirstPlayer;
            //game = new GameFactory().Create(GameType.AnimalShogi);
            game = new GameFactory().Create(GameType.FiveFiveShogi);
            OperationMode = OperationMode.SelectMoveFrom;

            MoveCommand = new DelegateCommand<object>(
                (param) =>
                {
                    if (param == null)
                        return;

                    if (OperationMode == OperationMode.SelectMoveFrom)
                    {
                        selectedMoveFrom = param as ISelectable;
                        selectedMoveFrom.IsSelected = true;
                        OperationMode = OperationMode.SelectMoveTo;

                        UpdateCanMove();
                    }
                    else if(OperationMode == OperationMode.SelectMoveTo)
                    {
                        var cell = param as CellViewModel;

                        if(cell != null && cell.CanMove)
                        {
                            MoveCommand move = null;

                            if(selectedMoveFrom is CellViewModel)
                            {
                                // [★成るかならないか選べるようにする]
                                var cellFrom = selectedMoveFrom as CellViewModel;
                                move = new BoardKomaMoveCommand(cellFrom.Koma.Player.ToDomain(), cell.Position, cellFrom.Position, false);
                            }
                            else if(selectedMoveFrom is HandKomaViewModel)
                            {
                                var hand = selectedMoveFrom as HandKomaViewModel;
                                move = new HandKomaMoveCommand(hand.Player.ToDomain(), cell.Position, hand.KomaType);

                            }

                            game.Play(move);
                        }

                        // [動けない位置の場合はキャンセルしすべて更新
                        OperationMode = OperationMode.SelectMoveFrom;
                        selectedMoveFrom = null;
                        Update();
                    }
                },
                (param) =>
                {
                    if (param == null)
                        return false;

                    if (OperationMode == OperationMode.SelectMoveFrom)
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
                    else if (OperationMode == OperationMode.SelectMoveTo)
                    {
                        return true;
                    }

                    return false;
                }
                );
            FirstPlayer = new PlayerViewModel(MoveCommand);
            SecondPlayer = new PlayerViewModel(MoveCommand);
            Update();
        }

        public void Update()
        {
            Board.Clear();
            FirstPlayer.Hands.Clear();
            SecondPlayer.Hands.Clear();

            for (int y = 0; y < game.Board.Height; y++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (int x = 0; x < game.Board.Width; x++)
                    row.Add(new CellViewModel() { Koma = null, Position = new BoardPosition(x, y), CanMove = false });
                Board.Add(row);
            }

            foreach (var koma in game.State.KomaList)
            {
                var boardPos = koma.Position as BoardPosition;

                if (boardPos != null)
                {
                    var cell = Board[boardPos.Y][boardPos.X];
                    cell.Koma = new KomaViewModel()
                    {
                        IsTransformed = koma.IsTransformed,
                        Name = koma.KomaType.Id,
                        Player = koma.Player == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer ? Player.FirstPlayer : Player.SecondPlayer,
                    };
                }

                var handPos = koma.Position as HandPosition;

                if (handPos != null)
                {
                    if (koma.Player == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer)
                        FirstPlayer.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.FirstPlayer});
                    else
                        SecondPlayer.Hands.Add(new HandKomaViewModel() { KomaName = koma.KomaType.Id, KomaType = koma.KomaType, Player = Player.SecondPlayer });
                }
            }
        }
        public void UpdateCanMove()
        {
            // [MEMO:ここでUpdate()(つまり、Board.Clear())してしまうと、持ち駒で同じ種類がある場合にどれをハイライトすべきか判別できなくなる]
            if (OperationMode != OperationMode.SelectMoveTo)
                return;

            var selectedCell = selectedMoveFrom as CellViewModel;
            var selectedHand = selectedMoveFrom as HandKomaViewModel;

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
                    cell.CanMove = moves.Any(x => x.ToPosition == cell.Position);
                }
            }

            MoveCommand.RaiseCanExecuteChanged();
        }
    }
}
