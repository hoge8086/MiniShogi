using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.GameFactorys;
using Shogi.Business.Domain.Model.Users;
using Shogi.Business.Domain.Model.AI;

namespace Shogi.Business.Application
{

    public class GameSet
    {
        public Dictionary<Player, User> Users;
        public Game Game;
        public GameType GameType;

        public GameSet(User firstPlayer, User secondPlayer, GameType gameType)
        {
            Users = new Dictionary<Player, User>();
            Users.Add(Player.FirstPlayer, firstPlayer);
            Users.Add(Player.SecondPlayer, secondPlayer);
            GameType = gameType;
            Game = new GameFactory().Create(GameType);
        }
        public void Reset()
        {
            Game = new GameFactory().Create(GameType);
        }
    }
    public interface GameListener
    {
        void OnStarted();
        void OnPlayed();
        void OnEnded();
    }

    public class GameService
    {
        private GameSet GameSet = null;
        private GameListener GameListener = null;

        public void Subscribe(GameListener listener)
        {
            GameListener = listener;
        }
        public void Start(GameSet gameSet)
        {
            //GameSet = new GameSet(firstPlayer, secondPlayer, gameType);
            GameSet = gameSet;
            GameListener?.OnStarted();
            Next();
        }
        public void Restart()
        {
            GameSet.Reset();
            GameListener?.OnStarted();
            Next();
        }

        public Game GetGame()
        {
            return GameSet.Game;
        }
        public User GetGameWinner()
        {
            if (!GameSet.Game.IsEnd)
                return null;

            return GameSet.Users[GameSet.Game.GameResult.Winner];
        }

        public void Play(MoveCommand moveCommand)
        {
            GameSet.Game.Play(moveCommand);
            GameListener?.OnPlayed();
            Next();
        }
        public void Next()
        {
            System.Diagnostics.Debug.WriteLine("----------------------");
            System.Diagnostics.Debug.WriteLine(GameSet.Game.ToString());
            if(GameSet.Game.IsEnd)
            {
                GameListener?.OnEnded();
                return;
            }

            if(GameSet.Users[GameSet.Game.State.TurnPlayer] is Human)
            {
                // [人]
                return;
            }
            else
            {
                // [AI]
                var ai = GameSet.Users[GameSet.Game.State.TurnPlayer] as AI;
                ai.Play(GameSet.Game);
                GameListener?.OnPlayed();
            }

            Next();
        }
    }
}
