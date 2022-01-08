using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
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
        public ReactiveProperty<uint> Width { get; set; }
        public ReactiveProperty<uint> Height { get; set; }
        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Width = new ReactiveProperty<uint>(3);
            Height = new ReactiveProperty<uint>(4);
            Width.Subscribe(x => UpdateView());
            Height.Subscribe(x => UpdateView());

            EditCellCommand = new ReactiveCommand<CellViewModel>();
            EditCellCommand.Subscribe(x =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(EditCellCondition), new EditCellCondition(x));
                navigationService.NavigateAsync(nameof(EditCellPage), param);
            });
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

        private static void UpdateBoardSize<T>(ObservableCollection<T> list, uint size) where T : new()
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
