using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;
using System.Threading;
using Shogi.Business.Domain.Model.GameTemplates;

namespace Shogi.Business.Application
{
    public class GameSet
    {
        public Dictionary<PlayerType, Player> Players;
        public Game Game;

        public GameSet(Player firstPlayer, Player secondPlayer, Game game)
        {
            Players = new Dictionary<PlayerType, Player>();
            Players.Add(PlayerType.FirstPlayer, firstPlayer);
            Players.Add(PlayerType.SecondPlayer, secondPlayer);
            Game = game;
            
            
        }
        public void Reset()
        {
            Game.Reset();
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
        public IGameTemplateRepository GameTemplateRepository;

        public GameService(IGameTemplateRepository gameTemplateRepository)
        {
            GameTemplateRepository = gameTemplateRepository;
        }

        public void Subscribe(GameListener listener)
        {
            lock(thisLock)
            {
                GameListener = listener;
            }
        }
        public void Start(Player firstPlayer, Player secondPlayer, string gameName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                //GameSet = new GameSet(firstPlayer, secondPlayer, gameType);
                GameTemplate template;
                if (gameName == null)
                    template = GameTemplateRepository.First();
                else
                    template = GameTemplateRepository.FindByName(gameName);
                GameSet = new GameSet(firstPlayer, secondPlayer, template.Game.Clone());
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

        public void Undo(Game.UndoType undoType)
        {
            lock (thisLock)
            {
                GameSet.Game.Undo(undoType);
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
            if(GameSet.Game.State.IsEnd)
            {
                GameListener?.OnEnded(GameSet.Game.State.GameResult.Winner);
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
