using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public class CreateGamePageViewModel : ViewModelBase
    {
        public GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> Game { get; set; }
        public ReactiveCommand<CellViewModel> EditCellCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }
        public ReactiveProperty<int> Width { get; set; }
        public ReactiveProperty<int> Height { get; set; }
        public string GameName { get; set; }
        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameName = "新しいゲーム";
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Width = new ReactiveProperty<int>(3);
            Height = new ReactiveProperty<int>(4);
            Width.Subscribe(x => UpdateView());
            Height.Subscribe(x => UpdateView());

            EditCellCommand = new ReactiveCommand<CellViewModel>();
            EditCellCommand.Subscribe(x =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(EditCellCondition), new EditCellCondition(x));
                navigationService.NavigateAsync(nameof(EditCellPage), param);
            });
            SaveCommand = new ReactiveCommand();
            SaveCommand.Subscribe(x =>
            {
                var command = new CreateGameCommand()
                {
                    Height = this.Height.Value,
                    Width = this.Width.Value,
                    Name = GameName,
                    ProhibitedMoves = new ProhibitedMoves(false, false, false, true),
                    TerritoryBoundary = 1,
                    WinCondition = WinConditionType.Checkmate,
                    TurnPlayer = PlayerType.Player1,
                    KomaList = CreateKomaList(),
                };
                App.CreateGameService.CreateGame(command);
                navigationService.GoBackToRootAsync();
            });
        }

        private List<Koma> CreateKomaList()
        {
            var komaList = new List<Koma>();

            for(int y = 0; y<Height.Value; y++)
            {
                for(int x = 0; x<Width.Value; x++)
                {
                    var cell = Game.Board.Cells[y][x];
                    if (cell.Koma.Value != null)
                    {
                        var koma = cell.Koma.Value;
                        komaList.Add(new Koma(
                            koma.PlayerType.Value,
                            koma.Name.Value,
                            new OnBoard(
                                new BoardPosition(x, y),
                                koma.IsTransformed.Value)
                            ));
                    }
                }
            }
            return komaList;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            UpdateView();
        }

        public void UpdateView()
        {
            UpdateBoardSize(Game.Board.Cells, Height.Value);
            Game.Board.Cells.ForEach(x => UpdateBoardSize(x, Width.Value));
        }

        private static void UpdateBoardSize<T>(ObservableCollection<T> list, int size) where T : new()
        {
            if(list.Count > size)
            {
                var delNum = list.Count - size;
                for(int i=0; i<delNum; i++)
                    list.RemoveAt(list.Count - i - 1);
            }

            if(list.Count < size)
            {
                var addNum = size - list.Count;
                for(int i=0; i<addNum; i++)
                    list.Add(new T());
            }
        }
    }
}
