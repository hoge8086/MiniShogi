using Prism.Commands;
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

namespace MiniShogiMobile.ViewModels
{
    public class CreateKomaListPageViewModel : NavigationViewModel
    {
        public AsyncReactiveCommand CreateCommand { get; }
        public AsyncReactiveCommand EditCommand { get; }
        public AsyncReactiveCommand DeleteCommand { get; }
        public ObservableCollection<string> KomaNameList { get; }
        public ReactiveProperty<string> SelectedKomaName { get; }
        public CreateKomaListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            var komaList = App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            KomaNameList = new ObservableCollection<string>(komaList.Keys);
            SelectedKomaName = new ReactiveProperty<string>();

            CreateCommand = new AsyncReactiveCommand();
            CreateCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "新規作成しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        //App.CreateGameService.KomaTypeRepository.RemoveById(SelectedKomaName.Value);
                    }
                });
            }).AddTo(this.Disposable);
            EditCommand = SelectedKomaName.Select(x => !string.IsNullOrEmpty(x)).ToAsyncReactiveCommand().AddTo(this.Disposable);
            EditCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "編集しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        //App.CreateGameService.KomaTypeRepository.RemoveById(SelectedKomaName.Value);
                    }
                });
            }).AddTo(this.Disposable);
            DeleteCommand = SelectedKomaName.Select(x => !string.IsNullOrEmpty(x)).ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        App.CreateGameService.KomaTypeRepository.RemoveById(SelectedKomaName.Value);
                        KomaNameList.Remove(SelectedKomaName.Value);
                        SelectedKomaName.Value = null;
                    }
                });
            }).AddTo(this.Disposable);
        }
    }
}
