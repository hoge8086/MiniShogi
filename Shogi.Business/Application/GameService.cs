using System;
using System.Text;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;
using System.Threading;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.PlayingGames;

namespace Shogi.Business.Application
{

    // [TODO:ゲームの進行をDomainServiceに移す]
    public interface GameListener
    {
        void OnStarted();
        void OnPlayed();
        void OnEnded(PlayerType winner);
    }
    public class GameService
    {
        private Object thisLock = new Object();
        private PlayingGame PlayingGame = null;
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
        public void Start(Player player1, Player player2, PlayerType firstTurnPlayer, string gameName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var template = GameTemplateRepository.FindByName(gameName);
                PlayingGame = new PlayingGame(player1, player2, firstTurnPlayer, template);
                GameListener?.OnStarted();
                Next(cancellation);
            }
        }
        public void Restart(CancellationToken cancellation)
        {
            lock (thisLock)
            {
                PlayingGame.Reset();
                GameListener?.OnStarted();
                Next(cancellation);
            }
        }

        public void Undo(Game.UndoType undoType)
        {
            lock (thisLock)
            {
                PlayingGame.Game.Undo(undoType);
            }
        }

        public Game GetGame()
        {
            // [MEMO:クローンを返すことでマルチスレッドでアクセス可能とする]
            // [MEMO:Game自身はGameでlockしている→★してないので注意]
            return PlayingGame.Game.Clone();
        }

        public void Play(MoveCommand moveCommand, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                PlayingGame.Game.Play(moveCommand);
                GameListener?.OnPlayed();
                Next(cancellation);
            }
        }
        private void Next(CancellationToken cancellation)
        {
            System.Diagnostics.Debug.WriteLine("----------------------");
            System.Diagnostics.Debug.WriteLine(PlayingGame.Game.ToString());
            if(PlayingGame.Game.State.IsEnd)
            {
                GameListener?.OnEnded(PlayingGame.Game.State.GameResult.Winner);
                return;
            }

            if(PlayingGame.TurnPlayer.IsHuman)//.Players[PlayingGame.Game.State.TurnPlayer] is Human)
            {
                // [人]
                return;
            }

            // [AI]
            var ai = PlayingGame.TurnPlayer as AI;
            ai.Play(PlayingGame.Game, cancellation);
            if (cancellation.IsCancellationRequested)
                return;
            GameListener?.OnPlayed();

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
            return PlayingGame.TurnPlayer.IsAI;
        }
    }
}
