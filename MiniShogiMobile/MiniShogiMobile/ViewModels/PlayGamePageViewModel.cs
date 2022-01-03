﻿using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Players;
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
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public interface ISelectable
    {
        void Select();
    }
    public interface IViewState
    {
        Task HandleAsync(PlayGamePageViewModel vm, ISelectable cell);
    }
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


    public class PlayGamePageViewModel : ViewModelBase, GameListener
    {
        private ReactiveProperty<IViewState> ViewState;
        public void ChangeState(IViewState state) => ViewState.Value = state;
        //public BoardViewModel<> Board { get; set; }
        public GameViewModel<CellPlayingViewModel, PlayerWithHandPlayingViewModel, HandKomaPlayingViewModel> Game { get; set; }
        public ReactiveCommand<ISelectable> MoveCommand { get; set; }

        public PlayGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            ViewState = new ReactiveProperty<IViewState>(new ViewStateWaiting());
            Game = new GameViewModel<CellPlayingViewModel, PlayerWithHandPlayingViewModel, HandKomaPlayingViewModel>();
            MoveCommand = new ReactiveCommand<ISelectable>();
            MoveCommand.Subscribe(async x =>
            {
                //var cell = x as CellPlayingViewModel;
                await ViewState.Value.HandleAsync(this, x);
            });
        }

        public async Task AppServiceCallCommandAsync(Action<GameService> action)
        {
            ChangeState(new ViewStateWaiting());
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
            // [MEMO:Clear()するとちらつきが発生するため、セルを挿入しなおさないでセルを更新する]
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
                else
                {
                    AddHnad(koma, koma.Player == PlayerType.Player1 ? Game.HandsOfPlayer1.Hands : Game.HandsOfPlayer2.Hands);
                }
            }

            Game.HandsOfPlayer1.IsCurrentTurn.Value = App.GameService.GetGame().State.TurnPlayer == PlayerType.Player1;
            Game.HandsOfPlayer2.IsCurrentTurn.Value = App.GameService.GetGame().State.TurnPlayer == PlayerType.Player2;
            UpdateHandView(Game.HandsOfPlayer1.Hands, PlayerType.Player1);
            UpdateHandView(Game.HandsOfPlayer2.Hands, PlayerType.Player2);
        }

        private void UpdateHandView(ObservableCollection<HandKomaPlayingViewModel> hands, PlayerType player)
        {
            // [MEMO:Clear()するとちらつきが発生＆順序が変わるため、順番を変えないように持ち手を更新する]

            // [初期化＆駒の数を0にする]
            hands.ForEach(x => {
                x.Num.Value = 0;
                x.IsSelected.Value = false;
            }) ;

            // [駒の数をカウントし直す]
            App.GameService.GetGame().State.KomaList.Where(x => !x.IsOnBoard && x.Player == player).ForEach(koma =>
            {
                var hand = hands.FirstOrDefault(x => x.Name == koma.TypeId);
                if (hand == null)
                    hands.Add(new HandKomaPlayingViewModel() { Name = koma.TypeId, Player = koma.Player});
                else
                    hand.Num.Value++;
            });

            // [存在しない駒は削除する]
            var nothings = hands.Where(x => x.Num.Value == 0).ToList();
            foreach (var item in nothings)
                hands.Remove(item);
        }
        private void AddHnad(Koma koma, ObservableCollection<HandKomaPlayingViewModel> hands)
        {
            var hand = hands.FirstOrDefault(x => x.Name == koma.TypeId);
            if (hand == null)
                hands.Add(new HandKomaPlayingViewModel() { Name = koma.TypeId, Player = koma.Player});
            else
                hand.Num.Value++;
        }

        public void OnStarted()
        {
            Device.InvokeOnMainThreadAsync(() =>
            {

                // [プレイヤー]
                Game.HandsOfPlayer1.Player.Value = App.GameService.GetPlayingGame().GerPlayer(PlayerType.Player1);
                Game.HandsOfPlayer1.Type.Value = PlayerType.Player1;
                Game.HandsOfPlayer1.TurnType.Value = App.GameService.GetGame().State.TurnPlayer == PlayerType.Player1 ? TurnType.FirstTurn : TurnType.SecondTurn;
                Game.HandsOfPlayer2.Player.Value = App.GameService.GetPlayingGame().GerPlayer(PlayerType.Player2);
                Game.HandsOfPlayer2.Type.Value = PlayerType.Player2;
                Game.HandsOfPlayer2.TurnType.Value = App.GameService.GetGame().State.TurnPlayer == PlayerType.Player2 ? TurnType.FirstTurn : TurnType.SecondTurn;

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
        public ReactiveProperty<int> Num { get; set; }
        public PlayerType Player { get; set; }

        public HandKomaViewModel()
        {
            Num = new ReactiveProperty<int>(1);
        }
    }

    public class HandKomaPlayingViewModel : HandKomaViewModel, ISelectable
    {
        public ReactiveProperty<bool> IsSelected { get; set; }

        public HandKomaPlayingViewModel()
        {
            IsSelected = new ReactiveProperty<bool>();
        }
        public void Select()
        {
            IsSelected.Value = true;
        }
    }
    public class HandViewModel<T> : BindableBase where T : HandKomaViewModel
    {
        public ObservableCollection<T> Hands { get; set; }

        public HandViewModel()
        {
            Hands = new ObservableCollection<T>();
        }
    }

    public enum TurnType
    {
        FirstTurn,
        SecondTurn,
    }

    public class PlayerWithHandPlayingViewModel : HandViewModel<HandKomaPlayingViewModel>
    {
        public ReactiveProperty<Player> Player { get; set; }
        public ReactiveProperty<PlayerType> Type { get; set; }
        public ReactiveProperty<TurnType> TurnType { get; set; }
        public  ReactiveProperty<bool> IsCurrentTurn { get; }
        public  ReadOnlyReactiveProperty<string> Name { get; }
        public  ReadOnlyReactiveProperty<string> TurnTypeName { get; }

        public string GetName() {
            if (Player.Value == null || Type.Value == null)
                return "";
            return Player.Value.IsAI ? "CPU" : (Type.Value == PlayerType.Player1 ? "P1" : "P2");
        }

        public PlayerWithHandPlayingViewModel()
        {
            Player = new ReactiveProperty<Player>();
            Type = new ReactiveProperty<PlayerType>();
            IsCurrentTurn = new ReactiveProperty<bool>();
            Name = Player.CombineLatest(Type, (x, y) => GetName()).ToReadOnlyReactiveProperty();
            TurnType = new ReactiveProperty<TurnType>();
            TurnTypeName = TurnType.Select(x => x == ViewModels.TurnType.FirstTurn ? "先手" : "後手").ToReadOnlyReactiveProperty();
        }

    }

    //public class GameViewModel<BoardCell, HandCell> where BoardCell : CellBaseViewModel where HandCell : HandKomaViewModel
    public class GameViewModel<BoardCell, Hands, HandCell> where BoardCell : CellBaseViewModel where Hands : HandViewModel<HandCell>, new()  where HandCell : HandKomaViewModel
    {
        public BoardViewModel<BoardCell> Board { get; set; }
        public Hands HandsOfPlayer1 { get; set; }
        public Hands HandsOfPlayer2 { get; set; }

        public GameViewModel()
        {
            HandsOfPlayer1 = new Hands();
            HandsOfPlayer2 = new Hands();
            Board = new BoardViewModel<BoardCell>();
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
