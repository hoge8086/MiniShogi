using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
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

    public class CreateGamePageViewModel : NavigationViewModel<CreateGameCondition>
    {
        public GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> Game { get; set; }
        public AsyncReactiveCommand<CellViewModel> EditCellCommand { get; set; }
        public AsyncReactiveCommand DeleteKomaCommand { get; set; }
        public AsyncReactiveCommand<CellViewModel> TapCellCommand { get; set; }
        public AsyncReactiveCommand EditSettingCommand {get;}
        public AsyncReactiveCommand SaveCommand { get; set; }
        public ReactiveProperty<CellViewModel> SelectedCell { get; set; }
        public AsyncReactiveCommand AddHandKomaCommand { get; set; }
        public ReadOnlyReactivePropertySlim<bool> IsSelectingKoma{ get; set; }

        private GameTemplate GameTemplate;

        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameTemplate = new GameTemplate();
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            SelectedCell = new ReactiveProperty<CellViewModel>();
            IsSelectingKoma = SelectedCell.Select(x => x != null && x.Koma.Value != null).ToReadOnlyReactivePropertySlim().AddTo(Disposable);
            TapCellCommand = new AsyncReactiveCommand<CellViewModel>();
            TapCellCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
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
                            SelectedCell.Value = x;
                            // 空セルなら駒を置くか聞く
                            bool doPutKoma = await pageDialogService.DisplayAlertAsync("確認", "駒を配置しますか?", "はい", "いいえ");
                            if (doPutKoma)
                            {
                                var selectedKomaType = await NavigateAsync<SelectKomaPageViewModel, SelectKomaConditions, string>(new SelectKomaConditions(null));
                                if(selectedKomaType.Success)
                                {
                                    var owner = ((GameTemplate.Height/ 2) > x.Position.Y) ? PlayerType.Player2 : PlayerType.Player1;
                                    var newKoma = new KomaViewModel(selectedKomaType.Data, owner, false);
                                    var result = await NavigateAsync<EditCellPageViewModel, KomaViewModel, KomaViewModel>(newKoma);
                                    if(result.Success)
                                    {
                                        //選択を解除
                                        SelectedCell.Value = null;
                                        //駒を配置
                                        x.Koma.Value = result.Data;
                                    }
                                }

                            }
                            else
                            {
                                // 駒を選択
                                SelectedCell.Value = null;
                            }
                        }
                        else
                        {
                            // 駒を選択
                            SelectedCell.Value = x;
                        }
                    }

                });
            }).AddTo(Disposable);
            EditCellCommand = new AsyncReactiveCommand<CellViewModel>();
            EditCellCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var result = await NavigateAsync<EditCellPageViewModel, KomaViewModel, KomaViewModel>(x.Koma.Value);
                    if(result.Success)
                    {
                        //選択を解除
                        SelectedCell.Value = null;
                        //駒を配置
                        x.Koma.Value = result.Data;
                    }
                });
            }).AddTo(this.Disposable);
            SaveCommand = new AsyncReactiveCommand();
            SaveCommand.Subscribe(async (x) =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    GameTemplate.KomaList = CreateKomaList();
                    App.CreateGameService.CreateGame(GameTemplate);
                    await navigationService .GoBackToRootAsync();
                });

            }).AddTo(this.Disposable);

            EditSettingCommand = new AsyncReactiveCommand();
            EditSettingCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var result = await NavigateAsync<EditGameSettingsPageViewModel, GameTemplate, GameTemplate>(GameTemplate);
                    if(result.Success)
                    {
                        GameTemplate = result.Data;
                        Game.Board.UpdateSize(GameTemplate.Height, GameTemplate.Width);
                        Title = GameTemplate.Name;
                    }
                });

            }).AddTo(this.Disposable);
            DeleteKomaCommand = new AsyncReactiveCommand();
            DeleteKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    // 駒を消し、選択を解除
                    SelectedCell.Value.Koma.Value = null;
                    SelectedCell.Value = null;
                });
            }).AddTo(this.Disposable);
            AddHandKomaCommand = new AsyncReactiveCommand();
            AddHandKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var KomaTypes =  App.CreateGameService.KomaTypeRepository.FindAll();
                    var addKoma = KomaTypes.FirstOrDefault(x => Game.HandsOfPlayer1.Hands.All(y => y.Name != x.Id));
                    if(addKoma != null)
                        Game.HandsOfPlayer1.Hands.Add(new HandKomaViewModel() { Name = addKoma.Id, Player= PlayerType.Player1});
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

        public override void Prepare(CreateGameCondition parameter)
        {
            if (parameter.GameName != null)
                GameTemplate = App.CreateGameService.GameTemplateRepository.FindByName(parameter.GameName);
            else
                GameTemplate = new GameTemplate();

            Game.Update(GameTemplate.Height, GameTemplate.Width, GameTemplate.KomaList);
            Title = GameTemplate.Name;
        }
    }
}
