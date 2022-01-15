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
        //private PlayingGame PlayingGame = null;
        private GameListener GameListener = null;
        public IGameTemplateRepository GameTemplateRepository;
        private ICurrentPlayingGameRepository CurrentPlayingGameRepository;
        private IPlayingGameRepository PlayingGameRepository;

        public GameService(IGameTemplateRepository gameTemplateRepository, ICurrentPlayingGameRepository currentPlayingGameRepository, IPlayingGameRepository playingGameRepository)
        {
            GameTemplateRepository = gameTemplateRepository;
            CurrentPlayingGameRepository = currentPlayingGameRepository;
            PlayingGameRepository = playingGameRepository;
        }

        public void Subscribe(GameListener listener)
        {
            lock(thisLock)
            {
                GameListener = listener;
            }
        }
        public void SaveCurrent(string playingName)
        {
            lock (thisLock)
            {
                var current = CurrentPlayingGameRepository.Current();

                if(PlayingGameRepository.FindByName(playingName) != null);
                    PlayingGameRepository.RemoveByName(playingName);

                current.ChangeName(playingName);
                PlayingGameRepository.Add(current);
            }
        }
        public void Continue(string playingName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var current = PlayingGameRepository.FindByName(playingName);
                CurrentPlayingGameRepository.Save(current);
                GameListener?.OnStarted();
                Next(current, cancellation);
            }
        }

        public void Start(Player player1, Player player2, PlayerType firstTurnPlayer, string gameName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var template = GameTemplateRepository.FindByName(gameName);
                var game = new GameFactory().Create(template, firstTurnPlayer);
                var playingGame = new PlayingGame(player1, player2, game, template);
                //CurrentPlayingGameRepository.Save(playingGame);
                GameListener?.OnStarted();
                Next(playingGame, cancellation);

                CurrentPlayingGameRepository.Save(playingGame);
            }
        }
        public void Restart(CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                playingGame.Reset();

                GameListener?.OnStarted();
                Next(playingGame, cancellation);

                CurrentPlayingGameRepository.Save(playingGame);
            }
        }

        public void Undo(Game.UndoType undoType)
        {
            lock (thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                playingGame.Game.Undo(undoType);

                CurrentPlayingGameRepository.Save(playingGame);
            }
        }

        public Game GetGame()
        {
            // [MEMO:クローンを返すことでマルチスレッドでアクセス可能とする]
            // [MEMO:Game自身はGameでlockしている→★してないので注意]
            var playingGame = CurrentPlayingGameRepository.Current();
            return playingGame.Game.Clone();
        }
        public PlayingGame GetPlayingGame()
        {
            // [MEMO:クローンを返すことでマルチスレッドでアクセス可能とする]
            // [MEMO:Game自身はGameでlockしている→★してないので注意]
            var playingGame = CurrentPlayingGameRepository.Current();
            return playingGame;
        }

        public void Play(MoveCommand moveCommand, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                playingGame.Game.Play(moveCommand);
                GameListener?.OnPlayed();
                Next(playingGame, cancellation);

                CurrentPlayingGameRepository.Save(playingGame);
            }
        }
        /// <summary>
        /// ゲーム進行はDomainService化する
        /// </summary>
        /// <param name="playingGame"></param>
        /// <param name="cancellation"></param>
        private void Next(PlayingGame playingGame, CancellationToken cancellation)
        {
            System.Diagnostics.Debug.WriteLine("----------------------");
            System.Diagnostics.Debug.WriteLine(playingGame.Game.ToString());
            if(playingGame.Game.State.IsEnd)
            {
                GameListener?.OnEnded(playingGame.Game.State.GameResult.Winner);
                return;
            }

            if(playingGame.TurnPlayer.IsHuman)//.Players[PlayingGame.Game.State.TurnPlayer] is Human)
            {
                // [人]
                return;
            }

            // [AI]
            var ai = playingGame.TurnPlayer as AI;
            ai.Play(playingGame.Game, cancellation);
            if (cancellation.IsCancellationRequested)
                return;
            GameListener?.OnPlayed();

            Next(playingGame, cancellation);
        }

        public void Resume(CancellationToken cancellation)
        {
            lock(thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                Next(playingGame, cancellation);
            }
        }

        // [TODO:リードモデルとしてGameSetをとれるようにして、このメソッドはなくす]
        //public bool IsTurnPlayerAI()
        //{
        //    return PlayingGame.TurnPlayer.IsAI;
        //}
    }
}
