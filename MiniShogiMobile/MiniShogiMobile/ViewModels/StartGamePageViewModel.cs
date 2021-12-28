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
using MiniShogiMobile.Utils;
using System.ComponentModel;

namespace MiniShogiMobile.ViewModels
{
    public class EnumPlayerTypeProvider : EnumListProvider<PlayerType> { }
    public enum PlayerType
    {
        [Description("あなた")]
        Human,
        [Description("AI")]
        AI,
    };

    public class EnumSelectFirstTurnPlayerProvider : EnumListProvider<SelectFirstTurnPlayer> { }
    public enum SelectFirstTurnPlayer
    {
        [Description("ランダム")]
        Random,
        [Description("プレイヤー1")]
        Player1,
        [Description("プレイヤー2")]
        Player2,
    };

    public class StartGamePageViewModel : ViewModelBase
    {
        public ReactiveCommand PlayGameCommand { get; set; }
        public ObservableCollection<string> GameNameList { get; set; }
        public ReactiveProperty<string> GameName { get; set; }
        public PlayperViewModel Player1 { get; set; }
        public PlayperViewModel Player2 { get; set; }
        public StartGamePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            GameName = new ReactiveProperty<string>();
            Player1 = new PlayperViewModel(PlayerType.Human);
            Player2 = new PlayperViewModel(PlayerType.AI, 5);

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

        public class PlayperViewModel : BindableBase
        {
            public ReactiveProperty<PlayerType> PlayerType { get; set; }
            public ReactiveProperty<int> AIThinkDepth { get; set; }

            public PlayperViewModel(PlayerType playerType, int depth = 0)
            {
                PlayerType = new ReactiveProperty<PlayerType>(playerType);
                AIThinkDepth = new ReactiveProperty<int>(depth);
            }
        }
    }
}
