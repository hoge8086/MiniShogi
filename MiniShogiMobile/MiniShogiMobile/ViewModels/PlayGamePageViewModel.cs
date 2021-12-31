using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Navigation;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniShogiMobile.ViewModels
{
    public class PlayGamePageViewModel : ViewModelBase
    {
        //public ObservableCollection<ObservableCollection<CellViewModel>> Board { get; set; }
        public BoardViewModel<CellPlayingViewModel> Board { get; set; }
        public ReactiveCommand<CellPlayingViewModel> MoveCommand { get; set; }

        public PlayGamePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Board = new ObservableCollection<ObservableCollection<CellViewModel>>();
            Board = new BoardViewModel<CellPlayingViewModel>();
            MoveCommand = new ReactiveCommand<CellPlayingViewModel>();
            MoveCommand.Subscribe(x =>
            {
                x.IsSelected.Value = !x.IsSelected.Value;
            });
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(PlayGameCondition)] as PlayGameCondition;
            if(param == null)
                throw new ArgumentException(nameof(PlayGameCondition));

            Title = param.Name;
            var cancelTokenSource = new CancellationTokenSource();
            //await Task.Run(() =>
            //{
                App.GameService.Start(param.Player1, param.Player2, param.FirstTurnPlayer, param.Name, cancelTokenSource.Token);
            //});
            //var game = App.GameService.GetGame();
            Update();
            //cancelTokenSource = null;


            // [MEMO:タスクが完了されるまでここは実行されない(AIThinkingのまま)]
            //UpdateOperationModeOnTaskFinished();
        }

        public void Update()
        {
            //Board.Clear();
            Board.Cells.Clear();
            //FirstPlayerHands.Hands.Clear();
            //SecondPlayerHands.Hands.Clear();

            for (int y = 0; y < App.GameService.GetGame().Board.Height; y++)
            {
                //var row = new ObservableCollection<CellViewModel>();
                var row = new ObservableCollection<CellPlayingViewModel>();
                for (int x = 0; x < App.GameService.GetGame().Board.Width; x++)
                    //row.Add(new CellViewModel() { Position = new BoardPosition(x, y), MoveCommands = null});
                    row.Add(new CellPlayingViewModel() { Position = new BoardPosition(x, y), MoveCommands = null});
                Board.Cells.Add(row);
            }

            foreach (var koma in App.GameService.GetGame().State.KomaList)
            {
                if (koma.BoardPosition != null)
                {
                    //var cell = Board[koma.BoardPosition.Y][koma.BoardPosition.X];
                    var cell = Board.Cells[koma.BoardPosition.Y][koma.BoardPosition.X];
                    cell.Koma.Value = new KomaViewModel()
                    {
                        IsTransformed = koma.IsTransformed,
                        Name = koma.TypeId,
                        PlayerType = koma.Player,
                    };
                }
                //else
                //{
                //    if (koma.Player == PlayerType.FirstPlayer)
                //        FirstPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.TypeId, KomaTypeId = koma.TypeId, Player = Player.FirstPlayer});
                //    else
                //        SecondPlayerHands.Hands.Add(new HandKomaViewModel() { KomaName = koma.TypeId, KomaTypeId = koma.TypeId, Player = Player.SecondPlayer });
                //}
            }
            
            //FirstPlayerHands.IsCurrentTurn = App.GameService.GetGame().State.TurnPlayer == PlayerType.FirstPlayer;
            //SecondPlayerHands.IsCurrentTurn = App.GameService.GetGame().State.TurnPlayer == PlayerType.SecondPlayer;
            //MoveCommand.RaiseCanExecuteChanged();
        }
    }
    public class HandViewModel
    {

    }
    public class BoardWithHandViewModel<T> where T : CellBaseViewModel
    {
        public BoardViewModel<T> Board { get; set; }
        public HandViewModel Player1 { get; set; }
        public HandViewModel Player2 { get; set; }
    }

    public class BoardViewModel<T> where T : CellBaseViewModel
    {
        public ObservableCollection<ObservableCollection<T>> Cells { get; set; }

        public BoardViewModel()
        {
            Cells = new ObservableCollection<ObservableCollection<T>>();
        }
    }

}
