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

        private GameTemplate GameTemplate;
        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameTemplate = new GameTemplate();
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();

            EditCellCommand = new ReactiveCommand<CellViewModel>();
            EditCellCommand.Subscribe(x =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(EditCellCondition), new EditCellCondition(x, GameTemplate.Height));
                navigationService.NavigateAsync(nameof(EditCellPage), param);
            }).AddTo(this.Disposable);
            SaveCommand = new ReactiveCommand();
            SaveCommand.Subscribe(x =>
            {
                CatchErrorWithMessage(() =>
                {
                    GameTemplate.KomaList = CreateKomaList();
                    App.CreateGameService.CreateGame(GameTemplate);
                    navigationService.GoBackToRootAsync();
                });

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

            for(int y = 0; y<GameTemplate.Height; y++)
            {
                for(int x = 0; x<GameTemplate.Width; x++)
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
            var navigationMode = parameters.GetNavigationMode();
            if (navigationMode == NavigationMode.New)
            {
                var param = parameters[nameof(CreateGameCondition)] as CreateGameCondition;
                if(param == null)
                    throw new ArgumentException(nameof(CreateGameCondition));

                if (param.GameName != null)
                    GameTemplate = App.CreateGameService.GameTemplateRepository.FindByName(param.GameName);
                else
                    GameTemplate = new GameTemplate();

                Game.Update(GameTemplate.Height, GameTemplate.Width, GameTemplate.KomaList);
            }
            else if (navigationMode == NavigationMode.Back)
            {
                Game.Board.UpdateSize(GameTemplate.Height, GameTemplate.Width);
            }

            Title = GameTemplate.Name;
        }
    }
}
