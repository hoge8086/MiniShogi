using Prism.Commands;
using Prism.Mvvm;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.GameFactorys;
using Shogi.Business.Domain.Model.Players;
using System;
using System.Collections.Generic;
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

        private GameType _gameType = GameType.AnimalShogi;
        public GameType GameType
        {
            get { return _gameType; }
            set { SetProperty(ref _gameType, value); }
        }

        public Shogi.Business.Domain.Model.Players.Player CreatePlayer(PlayerKind playerType, int aiThinkDepth)
        {
            if (playerType == PlayerKind.Human)
                return new Human();
            else
                return new NegaAlphaAI(aiThinkDepth);
        }
        public GameSet CreateGameSet()
        {
            return new GameSet(
                CreatePlayer(FirstPlayerType, FirstAIThinkDepth),
                CreatePlayer(SecondPlayerType, SecondAIThinkDepth),
                GameType);
        }
    }
}
