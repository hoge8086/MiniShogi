using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniShogiMobile.ViewModels
{
    public class ViewStateHumanThinkingForMoveFrom : IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, ISelectable fromCell)
        {
            var koma = GetKoma(fromCell, vm.PlayingGame.Game);
            if (koma == null || !vm.PlayingGame.Game.State.IsTurnPlayer(koma.Player))
                return;

            var moves = vm.PlayingGame.Game.CreateAvailableMoveCommand(koma);
            foreach(var row in vm.Game.Board.Cells)
            {
                foreach(var cell in row)
                {
                    var cellMoves = moves.Where(x => x.ToPosition == cell.Position).ToList();
                    if(cellMoves.Count() > 0)
                        cell.MoveCommands.Value = cellMoves;
                }
            }
            fromCell.Select();

            vm.ChangeState(new ViewStateHumanThinkingForMoveTo());
        }
        private Koma GetKoma(ISelectable cell, Game game)
        {
            if(cell is CellPlayingViewModel)
                return game.State.FindBoardKoma(((CellPlayingViewModel)cell).Position);
            if(cell is HandKomaViewModel)
                return game.State.FindHandKoma(((HandKomaViewModel)cell).Player, ((HandKomaViewModel)cell).KomaTypeId);

            throw new InvalidProgramException("MoveCommandに不明なパラメータが渡されました.");
        }
    }
    public class ViewStateHumanThinkingForMoveTo : IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, ISelectable cell)
        {

            var boardCell = cell as CellPlayingViewModel;
            if(boardCell != null && boardCell.CanMove.Value)
            {

                MoveCommand move = null;
                if(boardCell.MoveCommands.Value.Count == 1)
                {
                    move = boardCell.MoveCommands.Value[0];
                }
                else
                {
                    var doTransform = await vm.PageDialogService.DisplayAlertAsync("確認", "成りますか?", "はい", "いいえ");
                    move = boardCell.MoveCommands.Value.FirstOrDefault(x => x.DoTransform == doTransform);
                }
                await vm.AppServiceCallWithWaitAsync((service, cancelToken) =>
                {
                    service.Play(move, cancelToken);
                });
            }
            else
            {
                vm.UpdateView();
                vm.ChangeState(new ViewStateHumanThinkingForMoveFrom());
            }
        }
    }

    public class ViewStateGameStudying: IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, ISelectable cell)
        {
            return;
        }
    }

    public class ViewStateWaiting : IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, ISelectable cell)
        {
            return;
        }
    }
}
