using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MiniShogiMobile.ViewModels
{
    public interface IViewState
    {
        Task HandleAsync(PlayGamePageViewModel vm, CellPlayingViewModel cell);
    }
    public class ViewStateHumanThinkingForMoveFrom : IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, CellPlayingViewModel fromCell)
        {
            //Koma koma = selectedMoveSource.GetKoma(App.GameService.GetGame());
            var koma = App.GameService.GetGame().State.FindBoardKoma(fromCell.Position);
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
            fromCell.IsSelected.Value = true;

            vm.ChangeState(new ViewStateHumanThinkingForMoveTo());
        }
    }
    public class ViewStateHumanThinkingForMoveTo : IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, CellPlayingViewModel cell)
        {
            if(cell.CanMove.Value)
            {

                MoveCommand move = null;
                if(cell.MoveCommands.Value.Count == 1)
                {
                    move = cell.MoveCommands.Value[0];
                }
                else
                {
                    var doTransform = await vm.PageDialogService.DisplayAlertAsync("確認", "成りますか?", "Yes", "No");
                    move = cell.MoveCommands.Value.FirstOrDefault(x => x.DoTransform == doTransform);
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
        public async Task HandleAsync(PlayGamePageViewModel vm, CellPlayingViewModel cell)
        {
            return;
        }
    }

    public class ViewStateWaiting : IViewState
    {
        public async Task HandleAsync(PlayGamePageViewModel vm, CellPlayingViewModel cell)
        {
            return;
        }
    }


    public class PlayGamePageViewModel : ViewModelBase, GameListener
    {
        private ReactiveProperty<IViewState> ViewState;
        public void ChangeState(IViewState state) => ViewState.Value = state;
        //public BoardViewModel<> Board { get; set; }
        public GameViewModel<CellPlayingViewModel> Game { get; set; }
        public ReactiveCommand<CellPlayingViewModel> MoveCommand { get; set; }

        public PlayGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            ViewState = new ReactiveProperty<IViewState>();
            Game = new GameViewModel<CellPlayingViewModel>();
            MoveCommand = new ReactiveCommand<CellPlayingViewModel>();
            MoveCommand.Subscribe(async x =>
            {
                await ViewState.Value.HandleAsync(this, x);
            });
        }

        public async Task AppServiceCallCommandAsync(Action<GameService> action)
        {
            await Task.Run(() =>
            {
                action(App.GameService);
                if (App.GameService.GetGame().State.IsEnd)
                    ChangeState(new ViewStateGameEnd());
                else
                    ChangeState(new ViewStateHumanThinkingForMoveFrom());
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(PlayGameCondition)] as PlayGameCondition;
            if(param == null)
                throw new ArgumentException(nameof(PlayGameCondition));

            Title = param.Name;
            App.GameService.Subscribe(this);
            var cancelTokenSource = new CancellationTokenSource();
            await AppServiceCallCommandAsync(service =>
            {
                service.Start(param.Player1, param.Player2, param.FirstTurnPlayer, param.Name, cancelTokenSource.Token);
            });

        }

        public void UpdateView()
        {
            foreach(var row in Game.Board.Cells)
                foreach (var cell in row)
                    cell.Reset();

            foreach (var koma in App.GameService.GetGame().State.KomaList)
            {
                if (koma.BoardPosition != null)
                {
                    var cell = Game.Board.Cells[koma.BoardPosition.Y][koma.BoardPosition.X];
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

        public void OnStarted()
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                // [ボード初期化]
                // [MEMO:ボードのマス目の数はゲーム中は変わらないので、一度初期化するだけでよい]
                Game.Board.Cells.Clear();
                for (int y = 0; y < App.GameService.GetGame().Board.Height; y++)
                {
                    var row = new ObservableCollection<CellPlayingViewModel>();
                    for (int x = 0; x < App.GameService.GetGame().Board.Width; x++)
                        row.Add(new CellPlayingViewModel() { Position = new BoardPosition(x, y) });
                    Game.Board.Cells.Add(row);
                }
                UpdateView();
                Game.HandsOfPlayer2.Hands.Add(new HandKomaViewModel() { Name = "あ", Num = 2 });
                Game.HandsOfPlayer2.Hands.Add(new HandKomaViewModel() { Name = "い", Num = 1 });
            });
        }

        public void OnPlayed()
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                UpdateView();
            });
        }

        public void OnEnded(PlayerType winner)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var player = App.GameService.GetPlayingGame().GerPlayer(winner);
                PageDialogService.DisplayAlertAsync(
                     "ゲーム終了",
                     $"勝者: {player.Name}({winner.ToString()})",
                     "OK");
                //var winnerName = winner == PlayerType.Player1 ? FirstPlayerHands.Name : SecondPlayerHands.Name;
            });
        }
    }

    public class HandKomaViewModel : BindableBase
    {
        public string Name { get; set; }
        public int Num { get; set; }
    }
    public class HandViewModel : BindableBase
    {
        public ObservableCollection<HandKomaViewModel> Hands { get; set; }

        public HandViewModel()
        {
            Hands = new ObservableCollection<HandKomaViewModel>();
        }
    }
    public class GameViewModel<T> where T : CellBaseViewModel
    {
        public BoardViewModel<T> Board { get; set; }
        public HandViewModel HandsOfPlayer1 { get; set; }
        public HandViewModel HandsOfPlayer2 { get; set; }

        public GameViewModel()
        {
            HandsOfPlayer1 = new HandViewModel();
            HandsOfPlayer2 = new HandViewModel();
            Board = new BoardViewModel<T>();
        }

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
