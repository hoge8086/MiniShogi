using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using MiniShogiMobile.Conditions;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using MiniShogiMobile.Views;

namespace MiniShogiMobile.ViewModels
{
    public class StartGamePageViewModel : ViewModelBase
    {
        public ReactiveCommand PlayGameCommand { get; set; }
        public ObservableCollection<string> GameNameList { get; set; }
        public ReactiveProperty<string> GameName { get; set; }
        public StartGamePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            GameName = new ReactiveProperty<string>();

            PlayGameCommand = new ReactiveCommand();
            PlayGameCommand.Subscribe(() =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(PlayGameCondition), new PlayGameCondition(GameName.Value));
                navigationService.NavigateAsync(nameof(PlayGamePage), param);
            });

            GameNameList = new ObservableCollection<string>();
            foreach (var name in App.GameService.GameTemplateRepository.FindAllName())
                GameNameList.Add(name);
        }
    }
}
