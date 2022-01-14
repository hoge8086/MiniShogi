using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
        public ReactiveCommand EditSettingCommand {get;}
        public ReactiveCommand SaveCommand { get; set; }
        public ReactiveProperty<int> Width { get; set; }
        public ReactiveProperty<int> Height { get; set; }
        //public string GameName { get; set; }

        private GameTemplate GameTemplate;
        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

            //GameName = "新しいゲーム";
            GameTemplate = new GameTemplate();

            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Width = new ReactiveProperty<int>();
            Height = new ReactiveProperty<int>();
            Width.Subscribe(x => UpdateView()).AddTo(this.Disposable);
            Height.Subscribe(x => UpdateView()).AddTo(this.Disposable);

            EditCellCommand = new ReactiveCommand<CellViewModel>();
            EditCellCommand.Subscribe(x =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(EditCellCondition), new EditCellCondition(x, Height.Value));
                navigationService.NavigateAsync(nameof(EditCellPage), param);
            }).AddTo(this.Disposable);
            SaveCommand = new ReactiveCommand();
            SaveCommand.Subscribe(x =>
            {
                GameTemplate.KomaList = CreateKomaList();
                App.CreateGameService.CreateGame(GameTemplate);
                navigationService.GoBackToRootAsync();
            }).AddTo(this.Disposable);

            EditSettingCommand = new ReactiveCommand();
            EditSettingCommand.Subscribe(() =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(EditDetailGameSettingsCondition), new EditDetailGameSettingsCondition(GameTemplate));
                navigationService.NavigateAsync(nameof(EditGameSettingsPage), param);

            }).AddTo(this.Disposable);
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
            Title = GameTemplate.Name;
            Width.Value = GameTemplate.Width;
            Height.Value = GameTemplate.Height;
            UpdateView();
            var navigationMode = parameters.GetNavigationMode();
            //if (navigationMode == NavigationMode.New)
            //{
            //    // 次の画面へ進む場合
            //}
            //else
            //{
            //    // 前の画面へ戻る場合
            //}
        }

        public void UpdateView()
        {
            UpdateBoardSize(Game.Board.Cells, Height.Value);
            Game.Board.Cells.ForEach(x => UpdateBoardSize(x, Width.Value));
            for (int y = 0; y < Game.Board.Cells.Count; y++)
                for (int x = 0; x < Game.Board.Cells[y].Count; x++)
                    Game.Board.Cells[y][x].Position = new BoardPosition(x, y);

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
