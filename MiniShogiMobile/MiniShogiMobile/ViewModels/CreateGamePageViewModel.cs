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
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public class CreateGamePageViewModel : NavigationViewModel<CreateGameCondition>
    {
        public GameViewModel<CellViewModel<KomaViewModel>, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> Game { get; set; }
        public AsyncReactiveCommand EditCellCommand { get; set; }
        public AsyncReactiveCommand DeleteKomaCommand { get; set; }
        public AsyncReactiveCommand<CellViewModel<KomaViewModel>> TapCellCommand { get; set; }
        public AsyncReactiveCommand EditSettingCommand {get;}
        public AsyncReactiveCommand SaveCommand { get; set; }

        /// <summary>
        /// 選択中のCellViewModelかHandKomaViewModelのいずれかが入る
        /// </summary>
        public ReactiveProperty<BindableBase> Selected { get; set; }

        // TOOD:持ち駒関連のコマンドはHandKomaViewModelの派生クラスに移してプレイヤーごとに別コマンドにした方がすっきりするかも
        //      ただ、SelectedCellを購読する必要がある
        #region 持ち駒関連
        public AsyncReactiveCommand<PlayerType> AddHandKomaCommand { get; set; }
        public AsyncReactiveCommand<HandKomaViewModel> TapHandKomaCommand { get; set; }
        public AsyncReactiveCommand<PlayerType> TapKomadaiCommand { get; set; }
        // TODO:コマンドの有効無を表示と紐づけると分かり易い
        public ReadOnlyReactiveProperty<bool> CanMoveToPlayer1Komadai { get; set; }
        // TODO:コマンドの有効無を表示と紐づけると分かり易い
        public ReadOnlyReactiveProperty<bool> CanMoveToPlayer2Komadai { get; set; }
        #endregion
        public ReadOnlyReactivePropertySlim<bool> IsSelectingKoma{ get; set; }

        private GameTemplate GameTemplate;

        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameTemplate = new GameTemplate();
            Game = new GameViewModel<CellViewModel<KomaViewModel>, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Selected = new ReactiveProperty<BindableBase>();
            CanMoveToPlayer1Komadai = CreateCanMoveToKomadaiReactiveProperty(PlayerType.Player1);
            CanMoveToPlayer2Komadai = CreateCanMoveToKomadaiReactiveProperty(PlayerType.Player2);

            IsSelectingKoma = Selected.Select(x =>
            {
                if (x == null)
                    return false;

                if(x is CellViewModel<KomaViewModel> cell)
                    return cell.Koma.Value != null;

                return x is HandKomaViewModel;
            }).ToReadOnlyReactivePropertySlim().AddTo(Disposable);
            TapCellCommand = new AsyncReactiveCommand<CellViewModel<KomaViewModel>>();
            TapCellCommand.Subscribe(async tappedCell =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    // 駒がすでに選択中か?
                    if(Selected.Value != null)
                    {
                        if(tappedCell.Koma.Value == null)
                        {
                            // すでに駒が選択中の場合で、空セルをタップした場合は駒移動

                            if(Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                            {
                                // 盤上の駒→盤上の駒へ移動
                                tappedCell.Koma.Value = selectedCell.Koma.Value;
                                selectedCell.Koma.Value = null;
                            }
                            else if(Selected.Value is HandKomaViewModel selectedHandKoma)
                            {
                                // 持ち駒→盤上の駒へ移動
                                Game.GetHands(selectedHandKoma.Player).RemoveOne(selectedHandKoma.KomaTypeId);
                                tappedCell.Koma.Value = new KomaViewModel(selectedHandKoma.KomaTypeId, selectedHandKoma.Player, false);
                            }
                        }
                        // 選択を解除
                        Selected.Value = null;
                    }else
                    {
                        // 駒未選択の場合

                        // セルを選択
                        Selected.Value = tappedCell;

                        // 選択したセルに駒があるか?
                        if (tappedCell.Koma.Value == null)
                        {
                            // 駒がないい場合は駒を配置

                            Selected.Value = tappedCell;
                            var selectedKomaType = await NavigateAsync<SelectKomaPageViewModel, SelectKomaConditions, KomaTypeId>(new SelectKomaConditions(null, "配置する駒を選択してください"));

                            // 駒を選んだか?
                            if(selectedKomaType.Success)
                            {
                                var owner = ((GameTemplate.Height/ 2) > tappedCell.Position.Y) ? PlayerType.Player2 : PlayerType.Player1;
                                var newKoma = new KomaViewModel(selectedKomaType.Data, owner, false);
                                var result = await NavigateAsync<EditCellPageViewModel, KomaViewModel, KomaViewModel>(newKoma);
                                if(result.Success)
                                    //駒を配置
                                    tappedCell.Koma.Value = result.Data;
                            }

                            // キャンセル時は選択解除
                            Selected.Value = null;
                        }
                    }
                });
            }).AddTo(Disposable);
            TapHandKomaCommand = new AsyncReactiveCommand<HandKomaViewModel>();
            TapHandKomaCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    // MEMO:セル選択状態の場合はここに来ない
                    //      セル選択状態の場合は、駒台がハイライトされているため、持ち駒は選択できない

                    if(Selected.Value != null)
                        // 持ち駒を選択していたら選択解除
                        Selected.Value = null;
                    else
                        // 何も選択してないなら持ち駒を選択
                        Selected.Value = x;
                });
            });
            TapKomadaiCommand = new AsyncReactiveCommand<PlayerType>();
            TapKomadaiCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    if(Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                    {
                        // 盤上の駒→持ち駒へ移動
                        Game.GetHands(x).AddOne(selectedCell.Koma.Value.KomaTypeId.Value, x);
                        selectedCell.Koma.Value = null;
                    }
                    else if (Selected.Value is HandKomaViewModel handKoma)
                    {
                        // 相手の持ち駒→自分の持ち駒へ移動
                        Game.GetHands(handKoma.Player).RemoveOne(handKoma.KomaTypeId);
                        Game.GetHands(x).AddOne(handKoma.KomaTypeId, x);
                    }
                    Selected.Value = null;

                });
            }).AddTo(this.Disposable);
            EditCellCommand = Selected.Select(x =>
            {
                if (x == null)
                    return false;

                if(x is CellViewModel<KomaViewModel> cell)
                    return cell.Koma.Value != null;

                return false;
            }).ToAsyncReactiveCommand().AddTo(Disposable);
            EditCellCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var cell = Selected.Value as CellViewModel<KomaViewModel>;
                    var result = await NavigateAsync<EditCellPageViewModel, KomaViewModel, KomaViewModel>(cell.Koma.Value);
                    if(result.Success)
                    {
                        //駒を配置
                        cell.Koma.Value = result.Data;
                    }
                    //選択を解除
                    Selected.Value = null;
                });
            }).AddTo(this.Disposable);
            SaveCommand = new AsyncReactiveCommand();
            SaveCommand.Subscribe(async (x) =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doSave = await pageDialogService.DisplayAlertAsync("確認", "作成を完了しますか?\n完了後、ホーム画面に戻ります。", "はい", "いいえ");
                    if (doSave)
                    {
                        GameTemplate.KomaList = CreateKomaList();
                        App.CreateGameService.CreateGame(GameTemplate);
                        await navigationService .GoBackToRootAsync();
                    }
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
            DeleteKomaCommand = IsSelectingKoma.ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    // 駒削除、選択を解除

                    if (Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                        // 盤上の駒を削除
                        selectedCell.Koma.Value = null;

                    if (Selected.Value is HandKomaViewModel selectedHandKoma)
                        // 持ち駒を削除
                        Game.GetHands(selectedHandKoma.Player).RemoveOne(selectedHandKoma.KomaTypeId);

                    Selected.Value = null;
                });
            }).AddTo(this.Disposable);
            AddHandKomaCommand = new AsyncReactiveCommand<PlayerType>();
            AddHandKomaCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var selectedKomaType = await NavigateAsync<SelectKomaPageViewModel, SelectKomaConditions, KomaTypeId>(new SelectKomaConditions(null, "追加する駒を選択してください"));
                    if (selectedKomaType.Success)
                    {
                        Game.GetHands(x).AddOne(selectedKomaType.Data, x);
                    }
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
                            koma.KomaTypeId.Value,
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
        /// <summary>
        /// 駒台をハイライトするためのリアクティブプロパティの生成
        /// </summary>
        /// <param name="toPlayer"></param>
        /// <returns></returns>
        private ReadOnlyReactiveProperty<bool> CreateCanMoveToKomadaiReactiveProperty(PlayerType toPlayer)
        {
            return Selected.Select(x =>
            {
                // 盤の駒は常に手持ちへ移動可
                if(x is CellViewModel<KomaViewModel> cell)
                    return cell.Koma.Value != null;

                // 相手の持ち駒なら自身の手持ちへ移動可能
                if(x is HandKomaViewModel selectedHand)
                    return selectedHand.Player != toPlayer;

                return false;

            }).ToReadOnlyReactiveProperty()
            .AddTo(Disposable);
        }
    }
}
