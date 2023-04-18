using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Shogi.Business.Domain.Model.Komas;

namespace MiniShogiMobile.ViewModels
{
    public class CreateGameListPageViewModel : NavigationViewModel
    {
        #region ゲーム
        public ReactiveProperty<string> SelectedGameName { get; }
        public ObservableCollection<string> GameNameList { get; }
        public AsyncReactiveCommand EditGameCommand { get; set; }
        public AsyncReactiveCommand DeleteGameCommand {get;}
        public AsyncReactiveCommand CopyGameCommand { get; }
        public AsyncReactiveCommand CreateNewGameCommand {get;}

        #endregion

        #region 駒

        public AsyncReactiveCommand CreateKomaCommand { get; }
        public AsyncReactiveCommand EditKomaCommand { get; }
        public AsyncReactiveCommand DeleteKomaCommand { get; }
        public AsyncReactiveCommand CopyKomaCommand { get; }
        public ObservableCollection<KomaTypeId> KomaTypeIdList { get; }
        public ReactiveProperty<KomaTypeId> SelectedKomaTypeId { get; }
        #endregion

        public CreateGameListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            // ゲーム
            SelectedGameName = new ReactiveProperty<string>();
            GameNameList = new ObservableCollection<string>(App.CreateGameService.GameTemplateRepository.FindAllName());
            EditGameCommand = SelectedGameName.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            EditGameCommand.Subscribe(async () =>
            {
                await NavigateAsync<CreateGamePageViewModel, CreateGameCondition>(new CreateGameCondition(SelectedGameName.Value));
            }).AddTo(Disposable);

            DeleteGameCommand = SelectedGameName.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteGameCommand.Subscribe(async () =>
            {
                bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                if (doDelete && SelectedGameName.Value != null)
                {
                    App.CreateGameService.GameTemplateRepository.RemoveByName(SelectedGameName.Value);
                    GameNameList.Remove(SelectedGameName.Value);
                    SelectedGameName.Value = null;
                }

            }).AddTo(Disposable);
            CreateNewGameCommand = new AsyncReactiveCommand();
            CreateNewGameCommand.Subscribe(async () =>
            {
                await NavigateAsync<CreateGamePageViewModel, CreateGameCondition>(new CreateGameCondition(null));
            }).AddTo(Disposable);

            CopyGameCommand = SelectedGameName.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            CopyGameCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "コピーしますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        var gameNameList = App.GameService.GameTemplateRepository.FindAllName();
                        var result = await NavigateAsync<InputNamePopupPageViewModel, InputNameCondition, string>(
                            new InputNameCondition(
                                "名前を入力してください",
                                $"{SelectedGameName.Value}_コピー",
                                "OK",
                                null,
                                (n) => gameNameList.Contains(n) ? "既にその名前は使用しているため、使用できません。" : null));
                        if(result.Success)
                        {
                            App.CreateGameService.CopyGame(result.Data, SelectedGameName.Value);
                            GameNameList.Add(result.Data);
                        }
                    }
                });
            }).AddTo(this.Disposable);


            // 駒
            KomaTypeIdList = new ObservableCollection<KomaTypeId>();
            SelectedKomaTypeId = new ReactiveProperty<KomaTypeId>();
            UpdateKomaList();

            CreateKomaCommand = new AsyncReactiveCommand();
            CreateKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "新規作成しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        await NavigateAsync<CreateKomaPageViewModel, KomaTypeId, bool>(null);
                        UpdateKomaList();
                    }
                });
            }).AddTo(this.Disposable);
            EditKomaCommand = SelectedKomaTypeId.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            EditKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "編集しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        // MEMO:戻り値(bool)は使わないが、登録の完了を待つ必要があるため、bool値を持たせる。(戻り値を指定しないとPrism.NavigationExが待たないため)
                        await NavigateAsync<CreateKomaPageViewModel, KomaTypeId, bool>(SelectedKomaTypeId.Value);
                        UpdateKomaList();
                    }
                });
            }).AddTo(this.Disposable);
            DeleteKomaCommand = SelectedKomaTypeId.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        App.CreateGameService.KomaTypeRepository.RemoveById(SelectedKomaTypeId.Value);
                        SelectedKomaTypeId.Value = null;
                        UpdateKomaList();
                    }
                });
            }).AddTo(this.Disposable);
            CopyKomaCommand = SelectedKomaTypeId.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            CopyKomaCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "コピーしますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        App.CreateGameService.CopyKomaType(SelectedKomaTypeId.Value);
                        UpdateKomaList();
                    }
                });
            }).AddTo(this.Disposable);

        }
        private void UpdateKomaList()
        {
            var komaList = App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            KomaTypeIdList.Clear();
            foreach (var koma in komaList.Keys)
                KomaTypeIdList.Add(koma);
        }
    }
}
