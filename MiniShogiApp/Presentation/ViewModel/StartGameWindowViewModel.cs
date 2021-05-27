using Prism.Commands;
using Prism.Mvvm;
using Shogi.Business.Application;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.GameFactorys;
using Shogi.Business.Domain.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MiniShogiApp.Presentation.ViewModel
{
    public enum PlayerType
    {
        [Description("人")]
        Human,
        [Description("AI")]
        AI,
    }

    public class StartGameWindowViewModel : BindableBase
    {
        private PlayerType _firstPlayerType = PlayerType.Human;
        public PlayerType FirstPlayerType
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

        private PlayerType _secondPlayer  = PlayerType.AI;
        public PlayerType SecondPlayerType
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

        public Shogi.Business.Domain.Model.Users.User CreateUser(PlayerType playerType, int aiThinkDepth)
        {
            if (playerType == PlayerType.Human)
                return new Human();
            else
                return new NegaAlphaAI(aiThinkDepth);
        }
        public GameSet CreateGameSet()
        {
            return new GameSet(
                CreateUser(FirstPlayerType, FirstAIThinkDepth),
                CreateUser(SecondPlayerType, SecondAIThinkDepth),
                GameType);
        }
    }
}
