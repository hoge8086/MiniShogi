﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using Shogi.Business.Domain.Model.Komas;

namespace MiniShogiMobile.ViewModels
{
    public class CreateKomaListPageViewModel : NavigationViewModel
    {
        public AsyncReactiveCommand CreateCommand { get; }
        public AsyncReactiveCommand EditCommand { get; }
        public AsyncReactiveCommand DeleteCommand { get; }
        public ObservableCollection<KomaTypeId> KomaTypeIdList { get; }
        public ReactiveProperty<KomaTypeId> SelectedKomaTypeId { get; }
        public CreateKomaListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaTypeIdList = new ObservableCollection<KomaTypeId>();
            SelectedKomaTypeId = new ReactiveProperty<KomaTypeId>();
            UpdateKomaList();

            CreateCommand = new AsyncReactiveCommand();
            CreateCommand.Subscribe(async () =>
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
            EditCommand = SelectedKomaTypeId.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            EditCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "編集しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        await NavigateAsync<CreateKomaPageViewModel, KomaTypeId, bool>(SelectedKomaTypeId.Value);
                        UpdateKomaList();
                    }
                });
            }).AddTo(this.Disposable);
            DeleteCommand = SelectedKomaTypeId.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteCommand.Subscribe(async () =>
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
