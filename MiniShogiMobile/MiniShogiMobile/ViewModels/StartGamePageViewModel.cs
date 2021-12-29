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
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;

namespace MiniShogiMobile.ViewModels
{
    public class EnumPlayerTypeProvider : EnumListProvider<PlayerType> { }
    public class PlayerTypeConverter : EnumToDescriptionConverter<PlayerType> { }
    public enum PlayerType
    {
        [Description("あなた")]
        Human,
        [Description("AI")]
        AI,
    };

    public class EnumSelectFirstTurnPlayerProvider : EnumListProvider<SelectFirstTurnPlayer> { }
    public class SelectFirstTurnPlayerConverter : EnumToDescriptionConverter<SelectFirstTurnPlayer> { }
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
        public ReactiveProperty<SelectFirstTurnPlayer> FirstTurnPlayer { get; set; }

        public StartGamePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            GameNameList = new ObservableCollection<string>();
            foreach (var name in App.GameService.GameTemplateRepository.FindAllName())
                GameNameList.Add(name);

            GameName = new ReactiveProperty<string>(GameNameList.First());
            Player1 = new PlayperViewModel(PlayerType.Human);
            Player2 = new PlayperViewModel(PlayerType.AI, 5);
            FirstTurnPlayer = new ReactiveProperty<SelectFirstTurnPlayer>(SelectFirstTurnPlayer.Random);

            PlayGameCommand = new ReactiveCommand();
            PlayGameCommand.Subscribe(() =>
            {
                var firstTurnPlayer = FirstTurnPlayer.Value;
                if (firstTurnPlayer == SelectFirstTurnPlayer.Random)
                    firstTurnPlayer = new Random().Next(2) == 0 ? SelectFirstTurnPlayer.Player1 : SelectFirstTurnPlayer.Player2;
                var param = new NavigationParameters();
                param.Add(nameof(PlayGameCondition),
                    new PlayGameCondition(
                        GameName.Value,
                        firstTurnPlayer == SelectFirstTurnPlayer.Player1 ? Player1.CreatePlayer() : Player2.CreatePlayer(),
                        firstTurnPlayer == SelectFirstTurnPlayer.Player1 ? Player2.CreatePlayer() : Player1.CreatePlayer()
                    ));
                navigationService.NavigateAsync(nameof(PlayGamePage), param);
            });

        }

        public class PlayperViewModel : BindableBase
        {
            public ReactiveProperty<PlayerType> PlayerType { get; set; }
            public ReactiveProperty<int> AIThinkDepth { get; set; }

            public PlayperViewModel(PlayerType playerType, int depth = 1)
            {
                PlayerType = new ReactiveProperty<PlayerType>(playerType);
                AIThinkDepth = new ReactiveProperty<int>(depth);
            }

            public Player CreatePlayer()
            {
                if (PlayerType.Value == ViewModels.PlayerType.Human)
                    return new Human();
                else
                    return new NegaAlphaAI(AIThinkDepth.Value);
            }
        }
    }
}
