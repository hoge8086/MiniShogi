using Prism.Commands;
using Prism.Mvvm;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace MiniShogiApp.Presentation.ViewModel
{
    public enum PlayerKind
    {
        [Description("人")]
        Human,
        [Description("AI")]
        AI,
    }

    public class StartGameWindowViewModel : BindableBase
    {
        private PlayerKind _firstPlayerType = PlayerKind.Human;
        public PlayerKind FirstPlayerType
        {
            get { return _firstPlayerType; }
            set { SetProperty(ref _firstPlayerType, value); }
        }

        private int _firstAIThinkDepth = 6;
        public int FirstAIThinkDepth
        {
            get { return _firstAIThinkDepth; }
            set { SetProperty(ref _firstAIThinkDepth, value); }
        }

        private PlayerKind _secondPlayer  = PlayerKind.AI;
        public PlayerKind SecondPlayerType
        {
            get { return _secondPlayer; }
            set { SetProperty(ref _secondPlayer, value); }
        }

        private int _secondAIThinkDepth = 6;
        public int SecondAIThinkDepth
        {
            get { return _secondAIThinkDepth; }
            set { SetProperty(ref _secondAIThinkDepth, value); }
        }

        private string _gameType;
        public string GameType
        {
            get { return _gameType; }
            set { SetProperty(ref _gameType, value); }
        }

        public ObservableCollection<string> TemplateGameNameList { get; private set; }

        public Shogi.Business.Domain.Model.Players.Player CreatePlayer(PlayerKind playerType, int aiThinkDepth)
        {
            if (playerType == PlayerKind.Human)
                return new Human();
            else
                return new NegaAlphaAI(aiThinkDepth);
        }

        public Shogi.Business.Domain.Model.Players.Player FirstPlayer => CreatePlayer(FirstPlayerType, FirstAIThinkDepth);
        public Shogi.Business.Domain.Model.Players.Player SecondPlayer => CreatePlayer(SecondPlayerType, SecondAIThinkDepth);

        public StartGameWindowViewModel()
        {
            TemplateGameNameList = new ObservableCollection<string>(App.GameService.GameTemplateRepository.FindAllName());
            GameType = TemplateGameNameList[0];
        }
    }
}
