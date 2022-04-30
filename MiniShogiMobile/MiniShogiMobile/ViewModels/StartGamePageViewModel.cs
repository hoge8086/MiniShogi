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
using Shogi.Business.Domain.Model.PlayerTypes;
using Prism.Services;
using Shogi.Business.Domain.Model.GameTemplates;
using Reactive.Bindings.Extensions;
using Prism.NavigationEx;

namespace MiniShogiMobile.ViewModels
{
    public enum PlayerThinkingType
    {
        [Description("あなた")]
        Human,
        [Description("AI")]
        AI,
    };

    public class EnumPlayerTypeProvider : EnumListProvider<PlayerThinkingType> { }
    public class PlayerTypeConverter : EnumToDescriptionConverter<PlayerThinkingType> { }

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

    public class StartGamePageViewModel : NavigationViewModel
    {
        public AsyncReactiveCommand PlayGameCommand { get; set; }
        public ObservableCollection<string> GameNameList { get; set; }
        public ReactiveProperty<string> GameName { get; set; }
        public PlayperViewModel Player1 { get; set; }
        public PlayperViewModel Player2 { get; set; }
        public ReactiveProperty<SelectFirstTurnPlayer> FirstTurnPlayer { get; set; }

        public StartGamePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            GameNameList = new ObservableCollection<string>();
            foreach (var name in App.GameService.GameTemplateRepository.FindAllName())
                GameNameList.Add(name);

            GameName = new ReactiveProperty<string>(GameNameList.First());
            Player1 = new PlayperViewModel(PlayerThinkingType.Human);
            Player2 = new PlayperViewModel(PlayerThinkingType.AI, 5);
            FirstTurnPlayer = new ReactiveProperty<SelectFirstTurnPlayer>(SelectFirstTurnPlayer.Random);

            PlayGameCommand = new AsyncReactiveCommand();
            PlayGameCommand.Subscribe(async () =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(PlayGameCondition),
                    new PlayGameCondition(
                        PlayMode.NewGame,
                        GameName.Value,
                        Player1.CreatePlayer(),
                        Player2.CreatePlayer(),
                        GetFirstTurnPlayer()
                    ));
                await navigationService.NavigateAsync(nameof(PlayGamePage), param);
            }).AddTo(Disposable);;

        }

        private PlayerType GetFirstTurnPlayer()
        {
            switch(FirstTurnPlayer.Value)
            {
                case SelectFirstTurnPlayer.Player1:
                    return PlayerType.Player1;
                case SelectFirstTurnPlayer.Player2:
                    return PlayerType.Player2;
                case SelectFirstTurnPlayer.Random:
                    return new Random().Next(2) == 0 ? PlayerType.Player1 : PlayerType.Player2;
                default:
                    return PlayerType.Player1;
            }
        }

        public class PlayperViewModel : BindableBase
        {
            public ReactiveProperty<PlayerThinkingType> PlayerType { get; set; }
            public ReactiveProperty<int> AIThinkDepth { get; set; }

            public PlayperViewModel(PlayerThinkingType playerType, int depth = 1)
            {
                PlayerType = new ReactiveProperty<PlayerThinkingType>(playerType);
                AIThinkDepth = new ReactiveProperty<int>(depth);
            }

            public Player CreatePlayer()
            {
                if (PlayerType.Value == ViewModels.PlayerThinkingType.Human)
                    return new Human();
                else
                    return new NegaAlphaAI(AIThinkDepth.Value);
            }
        }
    }
}
