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
            //Koma koma = selectedMoveSource.GetKoma(App.GameService.GetGame());
            var koma = GetKoma(fromCell);//App.GameService.GetGame().State.FindBoardKoma(fromCell.Position);
            if (koma == null || !App.GameService.GetGame().State.IsTurnPlayer(koma.Player))
                return;

            var moves = App.GameService.GetGame().CreateAvailableMoveCommand(koma);
            foreach(var row in vm.Game.Board.Cells)
            {
                foreach(var cell in row)
                {
                    var cellMoves = moves.Where(x => x.ToPosition == cell.Position).ToList();
                    if(cellMoves.Count() > 0)
                        cell.MoveCommands.Value = cellMoves;
                }
            }
            //fromCell.IsSelected.Value = true;
            fromCell.Select();

            vm.ChangeState(new ViewStateHumanThinkingForMoveTo());
        }
        private Koma GetKoma(ISelectable cell)
        {
            if(cell is CellPlayingViewModel)
                return App.GameService.GetGame().State.FindBoardKoma(((CellPlayingViewModel)cell).Position);
            if(cell is HandKomaViewModel)
                return App.GameService.GetGame().State.FindHandKoma(((HandKomaViewModel)cell).Player, ((HandKomaViewModel)cell).Name);

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
                    var doTransform = await vm.PageDialogService.DisplayAlertAsync("確認", "成りますか?", "Yes", "No");
                    move = boardCell.MoveCommands.Value.FirstOrDefault(x => x.DoTransform == doTransform);
                }
                await vm.AppServiceCallCommandAsync(service =>
                {
                    service.Play(move, CancellationToken.None);
                });
            }
            else
            {
                vm.UpdateView();
                vm.ChangeState(new ViewStateHumanThinkingForMoveFrom());
            }
        }
    }

    public class ViewStateGameEnd: IViewState
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
