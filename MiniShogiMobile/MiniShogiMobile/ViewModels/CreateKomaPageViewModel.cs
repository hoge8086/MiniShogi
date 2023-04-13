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
using MiniShogiMobile.Utils;

namespace MiniShogiMobile.ViewModels
{
    public class EnumKomaTypeKindProvider : EnumListProvider<KomaTypeKind> { }
    public enum MoveType
    {
        None,
        Jump,
        RepeatableJump,
    }

    public class CreatingKomaViewModel
    {
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> IsTransformed { get; private set; } = new ReactiveProperty<bool>();
        public CreatingKomaViewModel(string name, bool isTransformed)
        {
            Name.Value = name;
            IsTransformed.Value = isTransformed;
        }
    }
    public class CellForCreateKomaViewModel : CellViewModel<CreatingKomaViewModel>
    {
        public ReactiveProperty<MoveType> MoveType { get; set; } = new ReactiveProperty<MoveType>(MiniShogiMobile.ViewModels.MoveType.None);
        public ReactiveProperty<bool> CanMove { get; set; } = new ReactiveProperty<bool>(false);
    }

    // MEMO:NavigationViewModelのResultは本来いらないが、Resultを指定しないと呼び出し元で待機しないので、仕方なくつける(要NavigationViewModelの改善)
    public class CreateKomaPageViewModel : NavigationViewModel<KomaTypeId, bool>
    {
        public AsyncReactiveCommand<CellForCreateKomaViewModel> ChangeMoveCommand { get; set; }
        public AsyncReactiveCommand SaveCommand { get; set; }
        public BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel> Board { get; set; }
        public BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel> PromotedBoard { get; set; }
        public ReactiveProperty<CreatingKomaViewModel> Koma { get; private set; }
        public ReactiveProperty<CreatingKomaViewModel> PromotedKoma { get; private set; }
        public ReactiveProperty<bool> CanBePromoted { get; private set; }
        public ReactiveProperty<KomaTypeKind> KomaTypeKind { get; private set; }

        private KomaType OldKomaType;
        private BoardPosition KomaPosition;

        private readonly int BoardSize = 9;
        public CreateKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Board = new BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel>();
            PromotedBoard = new BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel>();
            Koma = new ReactiveProperty<CreatingKomaViewModel>();
            PromotedKoma = new ReactiveProperty<CreatingKomaViewModel>();
            KomaTypeKind = new ReactiveProperty<KomaTypeKind>();
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
                //      なので、両方の盤を更新する(他に何か用方法があれば)
                UpdateCanMoveCellByRepeatableJump(Board);
                UpdateCanMoveCellByRepeatableJump(PromotedBoard);
            });
            SaveCommand = new AsyncReactiveCommand();
            SaveCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    if(OldKomaType != null)
                    {
                        bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "既にゲームへ配置済みの駒は変更されません。よろしいですか？", "はい", "いいえ");
                        if (!doDelete)
                            return;
                    }
                    App.CreateGameService.KomaTypeRepository.Replace(CreateKomaTypeFromBoard(), OldKomaType);
                    await GoBackAsync(true);
                });
            });
        }
        private KomaType CreateKomaTypeFromBoard()
        {
            return new KomaType(
                new KomaTypeId(Koma.Value.Name.Value, PromotedKoma.Value.Name.Value, KomaTypeKind.Value),
                new KomaMoves(CreateMovesFrom(Board)),
                CanBePromoted.Value ? new KomaMoves(CreateMovesFrom(PromotedBoard)) : null
                );
        }

        private List<IKomaMove> CreateMovesFrom(BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel> board)
        {
            var moves = new List<IKomaMove>();
            foreach(var moveOnCell in board.Cells.SelectMany(x => x).Where(x => x.MoveType.Value != MoveType.None))
            {
                moves.Add(new KomaMoveBase(moveOnCell.Position - KomaPosition, moveOnCell.MoveType.Value == MoveType.RepeatableJump));
            }
            return moves;
        }


        public override void Prepare(KomaTypeId parameter)
        {
            KomaType komaType;
            if (parameter != null)
            {
                komaType = App.CreateGameService.KomaTypeRepository.FindById(parameter);
                OldKomaType = komaType;
            }
            else
            {
                komaType = new KomaType();
                OldKomaType = null; 
            }
            KomaPosition = new BoardPosition(Board.Width / 2, Board.Height / 2);
            Koma.Value = new CreatingKomaViewModel(komaType.Id.Name, false);
            PromotedKoma.Value = new CreatingKomaViewModel(komaType.Id.PromotedName, true);
            KomaTypeKind.Value = komaType.Id.Kind;
            Board.Cells[KomaPosition.X][KomaPosition.Y].Koma.Value = Koma.Value;
            PromotedBoard.Cells[KomaPosition.X][KomaPosition.Y].Koma.Value = PromotedKoma.Value;
            SetMoves(Board, komaType.Moves.Moves);
            if(komaType.CanBeTransformed)
            {
                CanBePromoted.Value = true;
                SetMoves(PromotedBoard, komaType.TransformedMoves.Moves);
            }
        }

        private void SetMoves(BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel> board, List<IKomaMove> moves)
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

        private void UpdateCanMoveCellByRepeatableJump(BoardViewModel<CellForCreateKomaViewModel, CreatingKomaViewModel> board)
        {
            // 移動可能箇所をすべてクリア
            board.Cells.SelectMany(x => x).ForEach(x => x.CanMove.Value = false);

            // 移動箇所を再計算反映
            foreach(var move in CreateMovesFrom(board))
            {
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
