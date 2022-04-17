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

    public class CellGameCreatingViewModel : CellViewModel
    {
        public ReactiveProperty<bool> IsSelectedForMoving { get; set; }

        public CellGameCreatingViewModel()
        {
            IsSelectedForMoving = new ReactiveProperty<bool>(false);
        }
    }
    public class CreateGamePageViewModel : ViewModelBase
    {
        public GameViewModel<CellGameCreatingViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> Game { get; set; }
        public ReactiveCommand<CellGameCreatingViewModel> EditCellCommand { get; set; }
        public AsyncReactiveCommand DeleteKomaCommand { get; set; }
        public ReactiveCommand<CellGameCreatingViewModel> TapCellCommand { get; set; }
        public ReactiveCommand EditSettingCommand {get;}
        public ReactiveCommand SaveCommand { get; set; }

        private GameTemplate GameTemplate;

        // ReadOnlyReactivePropertyにしたかったが、行列追加で、監視対象のCellViewModelが増えるのに対応できないので、ReadOnlyにできなかった
        // IsKomaMoving = Game.Board.Cells.SelectMany(x => x)
        //                    .Select(x => x.IsSelectedForMoving)
        //                    .CombineLatestValuesAreAllFalse()
        //                    .Inverse()
        //                    .ToReadOnlyReactiveProperty()
        //                    .AddTo(this.Disposable);
        public ReactiveProperty<bool> IsKomaMoving { get; set; }
        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameTemplate = new GameTemplate();
            Game = new GameViewModel<CellGameCreatingViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            IsKomaMoving = new ReactiveProperty<bool>(false);
            TapCellCommand = new ReactiveCommand<CellGameCreatingViewModel>();
            TapCellCommand.Subscribe(async x =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    if(IsKomaMoving.Value)
                    {
                        // 駒選択中
                        var MovingCell = Game.Board.Cells.SelectMany(y => y).FirstOrDefault(y => y.IsSelectedForMoving.Value);
                        if(x.Koma.Value == null)
                        {
                            // 移動先が空なら駒を移動
                            x.Koma.Value = MovingCell.Koma.Value;
                            MovingCell.Koma.Value = null;
                        }
                        // 選択を解除
                        MovingCell.IsSelectedForMoving.Value = false;
                        IsKomaMoving.Value = false;
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
                            x.IsSelectedForMoving.Value = true;
                            IsKomaMoving.Value = true;
                        }
                    }

                });
            });
            EditCellCommand = new ReactiveCommand<CellGameCreatingViewModel>();
            EditCellCommand.Subscribe(async x =>
            {
                await CatchErrorWithMessageAsync(async () =>
                {
                    var param = new NavigationParameters();
                    param.Add(nameof(EditCellCondition), new EditCellCondition(x, GameTemplate.Height, true));
                    await navigationService.NavigateAsync(nameof(EditCellPage), param);

                    // 選択を解除
                    var MovingCell = Game.Board.Cells.SelectMany(y => y).FirstOrDefault(y => y.IsSelectedForMoving.Value);
                    MovingCell.IsSelectedForMoving.Value = false;
                    IsKomaMoving.Value = false;
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
                    var MovingCell = Game.Board.Cells.SelectMany(y => y).FirstOrDefault(y => y.IsSelectedForMoving.Value);
                    MovingCell.Koma.Value = null;
                    MovingCell.IsSelectedForMoving.Value = false;
                    IsKomaMoving.Value = false;
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
