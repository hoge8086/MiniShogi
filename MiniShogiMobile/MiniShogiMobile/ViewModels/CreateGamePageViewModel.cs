﻿using MarcTron.Plugin;
using MiniShogiMobile.Conditions;
using MiniShogiMobile.Service;
using MiniShogiMobile.Settings;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.AI;
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
        //public AsyncReactiveCommand EditCellCommand { get; set; }
        public AsyncReactiveCommand DeleteKomaCommand { get; set; }
        public AsyncReactiveCommand<CellViewModel<KomaViewModel>> TapCellCommand { get; set; }
        public AsyncReactiveCommand EditSettingCommand {get;}
        public AsyncReactiveCommand SaveCommand { get; set; }
        public AsyncReactiveCommand CopyKomaCommand { get; set; }
        public AsyncReactiveCommand ChangeKomaCommand { get; set; }
        public AsyncReactiveCommand ReverseKomaCommand { get; set; }
        public AsyncReactiveCommand RotateKomaCommand { get; set; }
        public AsyncReactiveCommand<BindableBase> ShowKomaInfoCommand { get; private set; }
        /// <summary>
        /// 選択中のCellViewModelかHandKomaViewModelのいずれかが入る
        /// </summary>
        public ReactiveProperty<BindableBase> Selected { get; set; }
        public ReadOnlyReactivePropertySlim<KomaViewModel> SelectedBoardKoma { get; set; }

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
        public ReadOnlyReactivePropertySlim<bool> IsSelectingKoma { get; set; }

        // TODO:複雑になってきたのでStateパターンを適用する
        public ReactiveProperty<bool> IsCopying { get; set; }
        public ReactiveProperty<string> GameTitle { get; set; }

        private GameTemplate GameTemplate;
        // 既存の盤上の駒種別と、現在のレポジトリの駒一覧を保持する(既存の盤上の駒種別はレポジトリから消してしまっている可能性があるため）
        private Dictionary<KomaTypeId, KomaType> KomaTypes;
        public CreateGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameTemplate = new GameTemplate();
            KomaTypes = App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            Game = new GameViewModel<CellViewModel<KomaViewModel>, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Selected = new ReactiveProperty<BindableBase>();
            IsCopying = new ReactiveProperty<bool>(false);
            CanMoveToPlayer1Komadai = CreateCanMoveToKomadaiReactiveProperty(PlayerType.Player1);
            CanMoveToPlayer2Komadai = CreateCanMoveToKomadaiReactiveProperty(PlayerType.Player2);
            GameTitle = new ReactiveProperty<string>();

            SelectedBoardKoma = Selected.Select(x =>
            {
                if (x == null)
                    return null;

                if (x is CellViewModel<KomaViewModel> cell)
                    return cell.Koma.Value;

                return null;
            }).ToReadOnlyReactivePropertySlim().AddTo(Disposable);
            IsSelectingKoma = Selected.Select(x =>
            {
                if (x == null)
                    return false;

                if (x is CellViewModel<KomaViewModel> cell)
                    return cell.Koma.Value != null;

                return x is HandKomaViewModel;
            }).ToReadOnlyReactivePropertySlim().AddTo(Disposable);

            // コピー中解除
            IsSelectingKoma.Where(x => !x).Subscribe(x =>
            {
                if (Selected.Value == null)
                    IsCopying.Value = false;
            }).AddTo(Disposable);

            TapCellCommand = new AsyncReactiveCommand<CellViewModel<KomaViewModel>>();
            TapCellCommand.Subscribe(async tappedCell =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    // 駒がすでに選択中か?
                    if (Selected.Value != null)
                    {
                        if (tappedCell.Koma.Value == null)
                        {
                            // すでに駒が選択中の場合で、空セルをタップした場合は駒移動

                            if (Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                            {
                                // 盤上の駒→盤上の駒へ移動 (コピー中の場合は元の駒を削除しない)
                                tappedCell.Koma.Value = new KomaViewModel(selectedCell.Koma.Value);
                                if (!IsCopying.Value)
                                    // 移動時は移動元の駒を削除
                                    selectedCell.Koma.Value = null;
                            }
                            else if (Selected.Value is HandKomaViewModel selectedHandKoma)
                            {
                                // 持ち駒→盤上の駒へ移動 (コピー中の場合は元の駒を削除しない)
                                tappedCell.Koma.Value = new KomaViewModel(selectedHandKoma.KomaTypeId, selectedHandKoma.Player, false);

                                if (!IsCopying.Value)
                                    // 移動時は移動元の手駒を削除
                                    Game.GetHands(selectedHandKoma.Player).RemoveOne(selectedHandKoma.KomaTypeId);
                            }
                            if (IsCopying.Value)
                                // コピー中はコピー先を選択中にする
                                Selected.Value = tappedCell;
                            else
                                Selected.Value = null;

                        } else
                        {
                            // 駒→別駒を選択する操作は選択、同じ駒は選択解除
                            if (Selected.Value == tappedCell)
                                Selected.Value = null;
                            else
                                Selected.Value = tappedCell;
                        }

                    } else
                    {
                        // 駒未選択の場合

                        // セルを選択
                        Selected.Value = tappedCell;

                        // 選択したセルに駒があるか?
                        if (tappedCell.Koma.Value == null)
                        {
                            // 駒がない場合は駒を配置

                            Selected.Value = tappedCell;
                            var selectedKomaType = await NavigateAsync<SelectKomaPopupPageViewModel, SelectKomaConditions, KomaTypeId>(new SelectKomaConditions(null, "配置する駒を選択してください"));

                            // 駒を選んだか?
                            if (selectedKomaType.Success)
                            {
                                var owner = ((GameTemplate.Height / 2) > tappedCell.Position.Y) ? PlayerType.Player2 : PlayerType.Player1;
                                var newKoma = new KomaViewModel(selectedKomaType.Data, owner, false);
                                //var result = await NavigateAsync<EditCellPopupPageViewModel, KomaViewModel, KomaViewModel>(newKoma);
                                //if(result.Success)
                                //    //駒を配置
                                //    tappedCell.Koma.Value = result.Data;
                                tappedCell.Koma.Value = newKoma;
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

                    if (Selected.Value != null)
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
                    if (Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                    {
                        // 盤上の駒→持ち駒へ移動
                        Game.GetHands(x).AddOne(selectedCell.Koma.Value.KomaTypeId.Value, x);
                        if (!IsCopying.Value)
                            selectedCell.Koma.Value = null;
                    }
                    else if (Selected.Value is HandKomaViewModel handKoma)
                    {
                        // 相手の持ち駒→自分の持ち駒へ移動
                        Game.GetHands(x).AddOne(handKoma.KomaTypeId, x);
                        if (!IsCopying.Value)
                            Game.GetHands(handKoma.Player).RemoveOne(handKoma.KomaTypeId);
                    }
                    if (!IsCopying.Value)
                        Selected.Value = null;

                });
            }).AddTo(this.Disposable);
            //EditCellCommand = Selected.Select(x =>
            //{
            //    if (x == null)
            //        return false;

            //    if(x is CellViewModel<KomaViewModel> cell)
            //        return cell.Koma.Value != null;

            //    return false;
            //}).ToAsyncReactiveCommand().AddTo(Disposable);
            //EditCellCommand.Subscribe(async x =>
            //{
            //    await this.CatchErrorWithMessageAsync(async () =>
            //    {
            //        var cell = Selected.Value as CellViewModel<KomaViewModel>;
            //        var result = await NavigateAsync<EditCellPopupPageViewModel, KomaViewModel, KomaViewModel>(cell.Koma.Value);
            //        if(result.Success)
            //        {
            //            //駒を配置
            //            cell.Koma.Value = result.Data;
            //        }
            //        //選択を解除
            //        Selected.Value = null;
            //    });
            //}).AddTo(this.Disposable);
            SaveCommand = IsCopying.Inverse().ToAsyncReactiveCommand().AddTo(Disposable);
            SaveCommand.Subscribe(async (x) =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    GameTemplate.KomaList = CreateKomaList();
                    GameTemplate.Name = GameTitle.Value;
                    var usingKomaTypeIds = Game.GetAllKomaTypeIds();
                    GameTemplate.KomaTypes = KomaTypes.Where(y => usingKomaTypeIds.Contains(y.Value.Id)).Select(y => y.Value).ToList();

#if DEBUG
                    // 評価値の表示デバッグ用
                    var game = new GameFactory().Create(GameTemplate, PlayerType.Player1);
                    var evaluator = new LossAndGainOfKomaEvaluator(game);
                    var eval = evaluator.Evaluate(game, 0, 0);
                    bool doSave = await pageDialogService.DisplayAlertAsync("確認", $"作成を完了しますか?\nこの将棋のAI対戦では最大{GameTemplate.MaxThinkingDepth}手読先までむことができます。(評価値：{eval.Value})", "はい", "いいえ");
#else
                    bool doSave = await pageDialogService.DisplayAlertAsync("確認", $"作成を完了しますか?\nこの将棋のAI対戦では最大{GameTemplate.MaxThinkingDepth}手読先までむことができます。", "はい", "いいえ");
#endif
                    if (doSave)
                    {

                        App.CreateGameService.SaveGameTemplate(GameTemplate);

                        if (CrossMTAdmob.Current.IsInterstitialLoaded())
                            CrossMTAdmob.Current.ShowInterstitial();

                        await navigationService.GoBackToRootAsync();
                    }
                });

            }).AddTo(this.Disposable);

            EditSettingCommand = IsCopying.Inverse().ToAsyncReactiveCommand().AddTo(Disposable);
            EditSettingCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var result = await NavigateAsync<EditGameSettingsPopupPageViewModel, GameTemplate, GameTemplate>(GameTemplate);
                    if (result.Success)
                    {
                        GameTemplate = result.Data;
                        Game.Board.UpdateSize(GameTemplate.Height, GameTemplate.Width);
                    }
                });

            }).AddTo(this.Disposable);
            DeleteKomaCommand = new[] { IsSelectingKoma, IsCopying.Inverse() }
                                .CombineLatestValuesAreAllTrue()
                                .ToAsyncReactiveCommand()
                                .AddTo(this.Disposable);
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
            AddHandKomaCommand = IsCopying.Inverse().ToAsyncReactiveCommand<PlayerType>().AddTo(Disposable);
            AddHandKomaCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    var selectedKomaType = await NavigateAsync<SelectKomaPopupPageViewModel, SelectKomaConditions, KomaTypeId>(new SelectKomaConditions(null, "追加する駒を選択してください"));
                    if (selectedKomaType.Success)
                    {
                        Game.GetHands(x).AddOne(selectedKomaType.Data, x);
                    }
                });
            }).AddTo(this.Disposable);

            CopyKomaCommand = IsSelectingKoma.ToAsyncReactiveCommand().AddTo(this.Disposable);
            CopyKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    if (IsCopying.Value)
                        Selected.Value = null;
                    else
                        IsCopying.Value = true;
                });
            }).AddTo(this.Disposable);

            //ChangeKomaCommand = SelectedBoardKoma.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            ChangeKomaCommand = new[] {
                                    SelectedBoardKoma.Select(x => x != null),
                                    IsCopying.Inverse()
                                }
                                .CombineLatestValuesAreAllTrue()
                                .ToAsyncReactiveCommand()
                                .AddTo(this.Disposable);
            ChangeKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {

                    if (Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                    {
                        if (selectedCell.Koma != null)
                        {
                            KomaTypeId oldkomaTypeId = selectedCell.Koma.Value.KomaTypeId.Value;
                            var selectedKomaType = await NavigateAsync<SelectKomaPopupPageViewModel, SelectKomaConditions, KomaTypeId>(new SelectKomaConditions(selectedCell.Koma.Value.KomaTypeId.Value, "変更する駒を選択してください"));

                            // 駒を選んだか?
                            if(selectedKomaType.Success && selectedKomaType.Data != oldkomaTypeId)
                            {
                                selectedCell.Koma.Value.KomaTypeId.Value = selectedKomaType.Data;

                                // TODO:あまりよくないので修正
                                // 駒種別(反転可否)が変わったため、選択駒を強制的に外すことで、コマンドのCanExecuteを再計算させる
                                var selected = Selected.Value;
                                Selected.Value = null;
                                Selected.Value = selected;

                                // 成り不可名駒で、既に成ってる場合は不成に戻す
                                if (!KomaTypes[selectedKomaType.Data].CanBeTransformed && selectedCell.Koma.Value.IsTransformed.Value)
                                    selectedCell.Koma.Value.IsTransformed.Value = false;
                            }

                        }
                    };
                });
            }).AddTo(this.Disposable);

            ReverseKomaCommand = new[] {
                                    SelectedBoardKoma.Select(x => x != null && KomaTypes[x.KomaTypeId.Value].CanBeTransformed),
                                    IsCopying.Inverse()
                                }
                                .CombineLatestValuesAreAllTrue()
                                .ToAsyncReactiveCommand()
                                .AddTo(this.Disposable);
            ReverseKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    if (Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                    {
                        if (selectedCell.Koma != null)
                            selectedCell.Koma.Value.IsTransformed.Value = !selectedCell.Koma.Value.IsTransformed.Value; 
                    };
                });
            }).AddTo(this.Disposable);

            RotateKomaCommand = new[] {
                                    SelectedBoardKoma.Select(x => x != null),
                                    IsCopying.Inverse()
                                }
                                .CombineLatestValuesAreAllTrue()
                                .ToAsyncReactiveCommand()
                                .AddTo(this.Disposable);
            RotateKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    if (Selected.Value is CellViewModel<KomaViewModel> selectedCell)
                    {
                        if (selectedCell.Koma != null)
                            selectedCell.Koma.Value.PlayerType.Value = selectedCell.Koma.Value.PlayerType.Value.Opponent;
                    };
                });
            }).AddTo(this.Disposable);

            ShowKomaInfoCommand = new AsyncReactiveCommand<BindableBase>();
            ShowKomaInfoCommand.Subscribe(async cell =>
            {
                KomaTypeId komaTypeId = null;
                if(cell is CellViewModel<KomaViewModel> boarCell)
                    komaTypeId = boarCell.Koma.Value?.KomaTypeId.Value;
                if (cell is HandKomaViewModel handCell)
                    komaTypeId = handCell.KomaTypeId;

                if (komaTypeId == null)
                    return;

                 await NavigateAsync<KomaInfoPopupPageViewModel, KomaType>(KomaTypes[komaTypeId]);
            }).AddTo(Disposable);
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
            komaList.AddRange(Game.HandsOfPlayer1.CreateKomaList());
            komaList.AddRange(Game.HandsOfPlayer2.CreateKomaList());
            return komaList;
        }

        public override void Prepare(CreateGameCondition parameter)
        {
            var adId = DependencyService.Get<ISettingService>().PrivateSetting.AdUnitIdForInterstitial;
            CrossMTAdmob.Current.LoadInterstitial(adId);
            if (parameter.GameName != null)
                GameTemplate = App.CreateGameService.GameTemplateRepository.FindByName(parameter.GameName);
            else
                GameTemplate = new GameTemplate();

            Game.Update(GameTemplate.Height, GameTemplate.Width, GameTemplate.KomaList);
            GameTitle.Value = GameTemplate.Name;
            // 駒種別の一覧に既存の盤上の駒種別を追加(レポジトリから削除した駒種別がある可能性があるため)
            GameTemplate.KomaTypes.ForEach(x =>
            {
                if (!KomaTypes.ContainsKey(x.Id))
                    KomaTypes.Add(x.Id, x);
            });

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
