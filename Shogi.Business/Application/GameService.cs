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
        public GameType gameType;

        public GameSet(User firstPlayer, User secondPlayer, GameType gameType)
        {

            Users = new Dictionary<Player, User>();
            Users.Add(Player.FirstPlayer, firstPlayer);
            Users.Add(Player.SecondPlayer, secondPlayer);
            Game = new GameFactory().Create(gameType);
        }
        public void Reset()
        {
            Game = new GameFactory().Create(gameType);
        }
    }
    public interface GameListener
    {
        void OnPlayed();
        void OnGameEnd();
    }

    public class GameService
    {
        private GameSet GameSet = null;
        private GameListener GameListener = null;
        public void Start(User firstPlayer, User secondPlayer, GameType gameType, GameListener gameListener)
        {
            GameSet = new GameSet(firstPlayer, secondPlayer, gameType);
            GameListener = gameListener;
            Next();
        }
        public void Restart()
        {
            GameSet.Reset();
            Next();
        }

        public Game GetGame()
        {
            return GameSet.Game;
        }

        public void Play(MoveCommand moveCommand)
        {
            GameSet.Game.Play(moveCommand);
            GameListener.OnPlayed();
            Next();
        }
        public void Next()
        {
            if(GameSet.Game.IsEnd)
            {
                GameListener.OnGameEnd();
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
                GameListener.OnPlayed();
            }

            Next();
        }
    }
}
