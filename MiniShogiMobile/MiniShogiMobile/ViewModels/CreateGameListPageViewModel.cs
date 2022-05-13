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

namespace MiniShogiMobile.ViewModels
{
    public class CreateGameListPageViewModel : NavigationViewModel
    {
        public ReactiveProperty<string> SelectedGameName { get; }
        public ObservableCollection<string> GameNameList { get; }
        public AsyncReactiveCommand EditCommand { get; set; }
        public AsyncReactiveCommand DeleteCommand {get;}
        public AsyncReactiveCommand CopyCommand { get; }
        public AsyncReactiveCommand CreateNewCommand {get;}
        public CreateGameListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            SelectedGameName = new ReactiveProperty<string>();
            GameNameList = new ObservableCollection<string>(App.CreateGameService.GameTemplateRepository.FindAllName());
            EditCommand = SelectedGameName.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            EditCommand.Subscribe(async () =>
            {
                await NavigateAsync<CreateGamePageViewModel, CreateGameCondition>(new CreateGameCondition(SelectedGameName.Value));
            }).AddTo(Disposable);

            DeleteCommand = SelectedGameName.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            DeleteCommand.Subscribe(async () =>
            {
                bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                if (doDelete && SelectedGameName.Value != null)
                {
                    App.CreateGameService.GameTemplateRepository.RemoveByName(SelectedGameName.Value);
                    GameNameList.Remove(SelectedGameName.Value);
                }

            }).AddTo(Disposable);
            CreateNewCommand = new AsyncReactiveCommand();
            CreateNewCommand.Subscribe(async () =>
            {
                await NavigateAsync<CreateGamePageViewModel, CreateGameCondition>(new CreateGameCondition(null));
            }).AddTo(Disposable);

            CopyCommand = SelectedGameName.Select(x => x != null).ToAsyncReactiveCommand().AddTo(this.Disposable);
            CopyCommand.Subscribe(async () =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "コピーしますか?", "はい", "いいえ");
                    if (doDelete)
                    {
                        App.CreateGameService.CopyGame(SelectedGameName.Value);
                    }
                });
            }).AddTo(this.Disposable);
        }
    }
}
