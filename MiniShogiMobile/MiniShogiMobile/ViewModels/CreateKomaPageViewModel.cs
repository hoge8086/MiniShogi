using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Boards;

namespace MiniShogiMobile.ViewModels
{
    public enum MoveType
    {
        None,
        Jump,
        RepeatableJump,
    }

    public class CellForCreateKomaViewModel : CellViewModel
    {
        public ReactiveProperty<MoveType> MoveType { get; set; } = new ReactiveProperty<MoveType>(MiniShogiMobile.ViewModels.MoveType.None);
        public ReactiveProperty<bool> CanMoveByRepeatableJump { get; set; } = new ReactiveProperty<bool>(false);
    }

    public class CreateKomaPageViewModel : NavigationViewModel<string>
    {
        public BoardViewModel<CellForCreateKomaViewModel> Board { get; set; }
        public BoardViewModel<CellForCreateKomaViewModel> PromotedBoard { get; set; }

        public ReactiveProperty<KomaViewModel> Koma { get; private set; }
        public ReactiveProperty<KomaViewModel> PromotedKoma { get; private set; }
        public ReactiveProperty<bool> CanBePromoted { get; private set; }

        private BoardPosition KomaPosition;

        private readonly int BoardSize = 7;
        public CreateKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Board = new BoardViewModel<CellForCreateKomaViewModel>();
            PromotedBoard = new BoardViewModel<CellForCreateKomaViewModel>();
            Koma = new ReactiveProperty<KomaViewModel>();
            PromotedKoma = new ReactiveProperty<KomaViewModel>();
            CanBePromoted = new ReactiveProperty<bool>(false);
            Board.UpdateSize(BoardSize, BoardSize);
            PromotedBoard.UpdateSize(BoardSize, BoardSize);
        }

        public override void Prepare(string parameter)
        {
            KomaType komaType;
            if (parameter != null)
                komaType = App.CreateGameService.KomaTypeRepository.FindById(parameter);
            else
                komaType = new KomaType();

            KomaPosition = new BoardPosition(Board.Width / 2, Board.Height / 2);
            Koma.Value = new KomaViewModel(komaType.Id, PlayerType.Player1, false);
            PromotedKoma.Value = new KomaViewModel(komaType.Id, PlayerType.Player1, true);
            Board.Cells[KomaPosition.X][KomaPosition.Y].Koma.Value = Koma.Value;
            PromotedBoard.Cells[KomaPosition.X][KomaPosition.Y].Koma.Value = PromotedKoma.Value;
            SetMoves(Board, komaType.Moves.Moves);
            if(komaType.CanBeTransformed)
            {
                CanBePromoted.Value = true;
                SetMoves(PromotedBoard, komaType.TransformedMoves.Moves);
            }
        }

        private void SetMoves(BoardViewModel<CellForCreateKomaViewModel> board, List<IKomaMove> moves)
        {
            foreach(var move in moves)
            {
                if(move is KomaMoveBase moveBase)
                {
                     var pos = KomaPosition.Add(moveBase.RelativeBoardPosition);
                     board.Cells[pos.Y][pos.X].MoveType.Value = moveBase.IsRepeatable ?  MoveType.RepeatableJump : MoveType.Jump;
                }
            }
            UpdateCanMoveCellByRepeatableJump(board);
        }

        private void UpdateCanMoveCellByRepeatableJump(BoardViewModel<CellForCreateKomaViewModel> board)
        {
            foreach(var repeatableJumpCell in board.Cells.SelectMany(x => x).Where(x => x.MoveType.Value == MoveType.RepeatableJump))
            {
                var move = new KomaMoveBase(repeatableJumpCell.Position - KomaPosition, true);
                var boardModel = new Shogi.Business.Domain.Model.Boards.Board(BoardSize, BoardSize);
                var movablePositions = move.GetMovableBoardPositions(PlayerType.Player1, KomaPosition, boardModel, new BoardPositions(), new BoardPositions());
                foreach(var pos in movablePositions.Positions)
                {
                    board.Cells[pos.Y][pos.X].CanMoveByRepeatableJump.Value = true;
                }
            }
        }

    }
}
