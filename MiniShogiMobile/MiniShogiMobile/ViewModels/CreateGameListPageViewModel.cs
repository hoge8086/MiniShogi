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

namespace MiniShogiMobile.ViewModels
{
    public class CreateGameListPageViewModel : NavigationViewModel
    {
        public ReactiveProperty<string> SelectedGameName { get; }
        public ObservableCollection<string> GameNameList { get; }
        public AsyncReactiveCommand EditCommand { get; set; }
        public AsyncReactiveCommand<string> DeleteCommand {get;}
        public AsyncReactiveCommand CreateNewCommand {get;}
        public CreateGameListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            SelectedGameName = new ReactiveProperty<string>();
            GameNameList = new ObservableCollection<string>(App.CreateGameService.GameTemplateRepository.FindAllName());
            EditCommand = new AsyncReactiveCommand();
            EditCommand.Subscribe(async () =>
            {
                await NavigateAsync<CreateGamePageViewModel, CreateGameCondition>(new CreateGameCondition(SelectedGameName.Value));
            }).AddTo(Disposable);

            DeleteCommand = new AsyncReactiveCommand<string>();
            DeleteCommand.Subscribe(async (name) =>
            {
                bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                if (doDelete && name != null)
                {
                    var temp = App.CreateGameService.GameTemplateRepository.FindByName(name);
                    App.CreateGameService.GameTemplateRepository.RemoveById(temp.Id);
                    GameNameList.Remove(name);
                }

            }).AddTo(Disposable);
            CreateNewCommand = new AsyncReactiveCommand();
            CreateNewCommand.Subscribe(async () =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(CreateGameCondition), new CreateGameCondition(null));
                await navigationService.NavigateAsync(nameof(CreateGamePage), param);
            }).AddTo(Disposable);
        }
    }
}
