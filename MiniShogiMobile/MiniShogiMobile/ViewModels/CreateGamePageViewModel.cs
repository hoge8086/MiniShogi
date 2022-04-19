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
        public AsyncReactiveCommand DeleteKomaCommand { get; set; }
        public ReactiveCommand<CellViewModel> TapCellCommand { get; set; }
        public ReactiveProperty<CellViewModel> SelectedCell { get; set; }
        public ReactiveCommand EditSettingCommand {get;}
        public ReactiveCommand SaveCommand { get; set; }

        private GameTemplate GameTemplate;

        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameTemplate = new GameTemplate();
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            //IsKomaMoving = new ReactiveProperty<bool>(false);
            SelectedCell = new ReactiveProperty<CellViewModel>();
            TapCellCommand = new ReactiveCommand<CellViewModel>();
            TapCellCommand.Subscribe(async x =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    if(SelectedCell.Value != null)
                    {
                        // 駒選択中
                        if(x.Koma.Value == null)
                        {
                            // 移動先が空なら駒を移動
                            x.Koma.Value = SelectedCell.Value.Koma.Value;
                            SelectedCell.Value.Koma.Value = null;
                        }
                        // 選択を解除
                        SelectedCell.Value = null;
                    }else
                    {
                        // 駒選択中ではない
                        if (x.Koma.Value == null)
                        {
                            // 空セルなら駒を置くか聞く
                            bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "駒を配置しますか?", "はい", "いいえ");
                            if (doDelete)
                            {
                                var param = new NavigationParameters();
                                param.Add(nameof(EditCellCondition), new EditCellCondition(x, GameTemplate.Height, false));
                                await navigationService.NavigateAsync(nameof(EditCellPage), param);
                            }
                        }
                        else
                        {
                            // 駒を選択
                            SelectedCell.Value = x;
                        }
                    }

                });
            });
            EditCellCommand = new ReactiveCommand<CellViewModel>();
            EditCellCommand.Subscribe(async x =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    var param = new NavigationParameters();
                    param.Add(nameof(EditCellCondition), new EditCellCondition(x, GameTemplate.Height, true));
                    await navigationService.NavigateAsync(nameof(EditCellPage), param);

                    // 選択を解除
                    SelectedCell.Value = null;
                });
            }).AddTo(this.Disposable);
            SaveCommand = new ReactiveCommand();
            SaveCommand.Subscribe(async (x) =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    GameTemplate.KomaList = CreateKomaList();
                    App.CreateGameService.CreateGame(GameTemplate);
                    await navigationService .GoBackToRootAsync();
                });

            }).AddTo(this.Disposable);

            EditSettingCommand = new ReactiveCommand();
            EditSettingCommand.Subscribe(async () =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    var param = new NavigationParameters();
                    param.Add(nameof(EditDetailGameSettingsCondition), new EditDetailGameSettingsCondition(GameTemplate));
                    await navigationService.NavigateAsync(nameof(EditGameSettingsPage), param);
                });

            }).AddTo(this.Disposable);
            //DeleteKomaCommand = new AsyncReactiveCommand<CellGameCreatingViewModel>();
            DeleteKomaCommand = new AsyncReactiveCommand();
            DeleteKomaCommand.Subscribe(async () =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    // 駒を消し、選択を解除
                    SelectedCell.Value.Koma.Value = null;
                    SelectedCell.Value = null;
                });
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
            await CatchErrorWithMessageAsync(async () =>
            {
                var navigationMode = parameters.GetNavigationMode();
                if (navigationMode == NavigationMode.New)
                {
                    var param = parameters[nameof(CreateGameCondition)] as CreateGameCondition;
                    if (param == null)
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

            });
        }
    }
}
