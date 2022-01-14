using MiniShogiMobile.Conditions;
using MiniShogiMobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class CreateGameListPageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> SelectedGameName { get; }
        public ObservableCollection<string> GameNameList { get; }
        public ReactiveCommand EditCommand { get; set; }
        public ReactiveCommand<string> DeleteCommand {get;}
        public ReactiveCommand CreateNewCommand {get;}
        public CreateGameListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            SelectedGameName = new ReactiveProperty<string>();
            GameNameList = new ObservableCollection<string>(App.CreateGameService.GameTemplateRepository.FindAllName());
            EditCommand = new ReactiveCommand();
            EditCommand.Subscribe(() =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(CreateGameCondition), new CreateGameCondition(SelectedGameName.Value));
                navigationService.NavigateAsync(nameof(CreateGamePage), param);
            }).AddTo(Disposable);

            DeleteCommand = new ReactiveCommand<string>();
            DeleteCommand.Subscribe(async (name) =>
            {
                bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                if (doDelete && name != null)
                {
                    App.CreateGameService.GameTemplateRepository.RemoveByName(name);
                    GameNameList.Remove(name);
                }

            }).AddTo(Disposable);
            CreateNewCommand = new ReactiveCommand();
            CreateNewCommand.Subscribe(() =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(CreateGameCondition), new CreateGameCondition(null));
                navigationService.NavigateAsync(nameof(CreateGamePage), param);
            }).AddTo(Disposable);
        }
    }
}
