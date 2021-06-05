using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.GameFactorys;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;
using System.Threading;

namespace Shogi.Business.Application
{

    public class GameSet
    {
        public Dictionary<PlayerType, Player> Players;
        public Game Game;
        public GameType GameType;

        public GameSet(Player firstPlayer, Player secondPlayer, GameType gameType)
        {
            Players = new Dictionary<PlayerType, Player>();
            Players.Add(PlayerType.FirstPlayer, firstPlayer);
            Players.Add(PlayerType.SecondPlayer, secondPlayer);
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
        void OnEnded(PlayerType winner);
    }

    public class GameService
    {
        private Object thisLock = new Object();

        private GameSet GameSet = null;
        private GameListener GameListener = null;

        public void Subscribe(GameListener listener)
        {
            lock(thisLock)
            {
                GameListener = listener;
            }
        }
        public void Start(GameSet gameSet, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                //GameSet = new GameSet(firstPlayer, secondPlayer, gameType);
                GameSet = gameSet;
                GameListener?.OnStarted();
                Next(cancellation);
            }
        }
        public void Restart(CancellationToken cancellation)
        {
            lock (thisLock)
            {
                GameSet.Reset();
                GameListener?.OnStarted();
                Next(cancellation);
            }
        }

        public Game GetGame()
        {
            // [MEMO:クローンを返すことでマルチスレッドでアクセス可能とする]
            // [MEMO:Game自身はGameでlockしている]
            return GameSet.Game.Clone();
        }

        public void Play(MoveCommand moveCommand, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                GameSet.Game.Play(moveCommand);
                GameListener?.OnPlayed();
                Next(cancellation);
            }
        }
        private void Next(CancellationToken cancellation)
        {
            System.Diagnostics.Debug.WriteLine("----------------------");
            System.Diagnostics.Debug.WriteLine(GameSet.Game.ToString());
            if(GameSet.Game.IsEnd)
            {
                GameListener?.OnEnded(GameSet.Game.GameResult.Winner);
                return;
            }

            if(GameSet.Players[GameSet.Game.State.TurnPlayer] is Human)
            {
                // [人]
                return;
            }
            else
            {
                // [AI]
                var ai = GameSet.Players[GameSet.Game.State.TurnPlayer] as AI;
                ai.Play(GameSet.Game, cancellation);
                if (cancellation.IsCancellationRequested)
                    return;
                GameListener?.OnPlayed();
            }

            Next(cancellation);
        }

        public void Resume(CancellationToken cancellation)
        {
            lock(thisLock)
            {
                Next(cancellation);
            }
        }

        // [TODO:リードモデルとしてGameSetをとれるようにして、このメソッドはなくす]
        public bool IsTurnPlayerAI()
        {
            return GameSet.Players[GameSet.Game.State.TurnPlayer] is AI;
        }
    }
}
