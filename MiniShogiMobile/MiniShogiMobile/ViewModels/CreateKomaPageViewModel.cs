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
using Xamarin.Forms.Internals;

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
        public ReactiveProperty<bool> CanMove { get; set; } = new ReactiveProperty<bool>(false);
    }

    public class CreateKomaPageViewModel : NavigationViewModel<string>
    {
        public BoardViewModel<CellForCreateKomaViewModel> Board { get; set; }
        public BoardViewModel<CellForCreateKomaViewModel> PromotedBoard { get; set; }
        public AsyncReactiveCommand<CellForCreateKomaViewModel> ChangeMoveCommand { get; set; }
        public ReactiveProperty<KomaViewModel> Koma { get; private set; }
        public ReactiveProperty<KomaViewModel> PromotedKoma { get; private set; }
        public ReactiveProperty<bool> CanBePromoted { get; private set; }

        private BoardPosition KomaPosition;

        private readonly int BoardSize = 9;
        public CreateKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Board = new BoardViewModel<CellForCreateKomaViewModel>();
            PromotedBoard = new BoardViewModel<CellForCreateKomaViewModel>();
            Koma = new ReactiveProperty<KomaViewModel>();
            PromotedKoma = new ReactiveProperty<KomaViewModel>();
            CanBePromoted = new ReactiveProperty<bool>(false);
            Board.UpdateSize(BoardSize, BoardSize);
            PromotedBoard.UpdateSize(BoardSize, BoardSize);

            ChangeMoveCommand = new AsyncReactiveCommand<CellForCreateKomaViewModel>();
            ChangeMoveCommand.Subscribe(async x =>
            {
                if (x.Position == KomaPosition)
                    return;

                x.MoveType.Value = (MoveType)(((int)x.MoveType.Value + 1) % (int)(MoveType.RepeatableJump + 1));

                // MEMO:不成と成りでコマンドを分けた方がよいがResourcesでControlTemplateを使ってるのでコマンドを分けれない
                //      なので、両方の盤を更新する
                UpdateCanMoveCellByRepeatableJump(Board);
                UpdateCanMoveCellByRepeatableJump(PromotedBoard);
            });
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
            // 移動可能箇所をすべてクリア
            board.Cells.SelectMany(x => x).ForEach(x => x.CanMove.Value = false);

            // 移動箇所を再計算反映
            foreach(var moveOnCell in board.Cells.SelectMany(x => x).Where(x => x.MoveType.Value != MoveType.None))
            {
                var move = new KomaMoveBase(moveOnCell.Position - KomaPosition, moveOnCell.MoveType.Value == MoveType.RepeatableJump);
                var boardModel = new Shogi.Business.Domain.Model.Boards.Board(BoardSize, BoardSize);
                var movablePositions = move.GetMovableBoardPositions(PlayerType.Player1, KomaPosition, boardModel, new BoardPositions(), new BoardPositions());
                foreach(var pos in movablePositions.Positions)
                {
                    board.Cells[pos.Y][pos.X].CanMove.Value = true;
                }
            }
        }

    }
}
