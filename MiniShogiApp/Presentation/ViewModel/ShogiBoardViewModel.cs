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

namespace MiniShogiApp.Presentation.ViewModel
{

    public enum OperationMode
    {
        SelectMoveFrom,
        SelectMoveTo,
        Wait,
    };

    public class ShogiBoardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public DelegateCommand<CellViewModel> MoveCommand { get; set; }
        public ObservableCollection<ObservableCollection<CellViewModel>> Board { get; set; } = new ObservableCollection<ObservableCollection<CellViewModel>>();

        public OperationMode OperationMode { get; private set; }
        // [★後で直す]
        private object selectedMoveFrom;

        private Game game;
        public ShogiBoardViewModel()
        {
            //game = new GameFactory().Create(GameType.AnimalShogi);
            game = new GameFactory().Create(GameType.FiveFiveShogi);
            OperationMode = OperationMode.SelectMoveFrom;

            MoveCommand = new DelegateCommand<CellViewModel>(
                (cell) =>
                {
                    if (cell == null)
                        return;

                    if (OperationMode == OperationMode.SelectMoveFrom)
                    {
                        selectedMoveFrom = cell;
                        OperationMode = OperationMode.SelectMoveTo;
                    }
                    else if(OperationMode == OperationMode.SelectMoveTo)
                    {
                        if(cell.CanMove)
                        {
                            MoveCommand move = null;

                            if(selectedMoveFrom is CellViewModel)
                            {
                                // [★成るかならないか選べるようにする]
                                var cellFrom = selectedMoveFrom as CellViewModel;
                                move = new BoardKomaMoveCommand(cellFrom.Koma.Player.ToDomain(), cell.Position, cellFrom.Position, false);
                            }

                            game.Play(move);
                        }

                        // [動けない位置の場合はキャンセル]
                        OperationMode = OperationMode.SelectMoveFrom;
                        selectedMoveFrom = null;
                    }
                    Update();
                },
                (cell) =>
                {
                    if (cell == null)
                        return false;

                    if (OperationMode == OperationMode.SelectMoveFrom)
                    {
                        if (cell.Koma == null)
                            return false;

                        return game.State.TurnPlayer == cell.Koma.Player.ToDomain();
                    }
                    else if (OperationMode == OperationMode.SelectMoveTo)
                    {
                        return true;
                    }

                    return false;
                }
                );
            Update();
        }

        public void Update()
        {
            Board.Clear();

            for(int y=0; y<game.Board.Height; y++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (int x = 0; x < game.Board.Width; x++)
                    row.Add(new CellViewModel() { Koma = null, Position = new BoardPosition(x, y), CanMove = false }) ;
                Board.Add(row);
            }

            foreach(var koma in game.State.KomaList)
            {
                var boardPos = koma.Position as BoardPosition;

                if(boardPos != null)
                {
                    var cell = Board[boardPos.Y][boardPos.X];
                    cell.Koma = new KomaViewModel()
                    {
                        IsTransformed = koma.IsTransformed,
                        Name = koma.KomaType.Id,
                        Player = koma.Player == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer ? Player.FirstPlayer : Player.SecondPlayer,
                    };
                }
            }

            if(OperationMode == OperationMode.SelectMoveTo)
            {
                var koma = game.State.FindBoardKoma(((CellViewModel)selectedMoveFrom).Position);
                var moves = game.CreateAvailableMoveCommand(koma);
                foreach(var row in Board)
                {
                    foreach(var cell in row)
                    {
                        cell.CanMove = moves.Any(x => x.ToPosition == cell.Position);
                    }
                }
                var selectedCell = selectedMoveFrom as CellViewModel;
                if (selectedCell != null)
                    Board[selectedCell.Position.Y][selectedCell.Position.X].IsSelected = true;

            }
        }
    }
}
