﻿using System;
using System.Text;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;
using System.Threading;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.PlayingGames;
using System.Collections.Generic;

namespace Shogi.Business.Application
{

    // [TODO:ゲームの進行をDomainServiceに移す]
    // [MEMO:ApplicationServiceからGetGmae()やGetPlayingGmae()で直接取れないようにすることで、マルチスレッドアクセスを可能にする]
    // [     Clone()したPlayingGameが返る]
    public interface GameListener : IProgress<ProgressInfoOfAIThinking>
    {
        void OnStarted(PlayingGame playingGame);
        void OnPlayed(PlayingGame playingGame, MoveCommand moveCommand);
        void OnEnded(PlayerType winner);
    }
    public class GameService
    {
        private Object thisLock = new Object();
        private GameListener GameListener = null;
        public IGameTemplateRepository GameTemplateRepository;
        public ICurrentPlayingGameRepository CurrentPlayingGameRepository;
        public IPlayingGameRepository PlayingGameRepository;

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

                if(PlayingGameRepository.FindByName(playingName) != null)
                    PlayingGameRepository.RemoveByName(playingName);

                current.ChangeName(playingName);
                PlayingGameRepository.Add(current);
            }
        }
        public void Continue(string playingName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                if(playingName != null)
                {
                    var continueGame = PlayingGameRepository.FindByName(playingName);
                    CurrentPlayingGameRepository.Save(continueGame);
                }

                var current = CurrentPlayingGameRepository.Current();
                GameListener?.OnStarted(current.Clone());
                Next(current, cancellation);
                CurrentPlayingGameRepository.Save(current);
            }
        }

        public void Start(List<Player> players, PlayerType firstTurnPlayer, string gameName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var template = GameTemplateRepository.FindByName(gameName);
                var game = new GameFactory().Create(template, firstTurnPlayer);
                var playingGame = new PlayingGame(players, game, template);
                GameListener?.OnStarted(playingGame.Clone());
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

                GameListener?.OnStarted(playingGame.Clone());
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

        public PlayingGame GetCurrentPlayingGame()
        {
            lock (thisLock)
            {
                return CurrentPlayingGameRepository.Current().Clone();
            }
        }

        public void Play(MoveCommand moveCommand, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                playingGame.Game.Play(moveCommand);
                GameListener?.OnPlayed(playingGame.Clone(), moveCommand);
                CurrentPlayingGameRepository.Save(playingGame);
                Next(playingGame, cancellation);
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

            if(playingGame.TurnPlayer.IsHuman)
            {
                return;
            }

            var moveCommand = playingGame.TurnPlayer.Computer.Play(playingGame.Game, cancellation, GameListener);
            GameListener?.OnPlayed(playingGame.Clone(), moveCommand);
            CurrentPlayingGameRepository.Save(playingGame);

            cancellation.ThrowIfCancellationRequested();

            Next(playingGame, cancellation);
        }

        public void Resume(CancellationToken cancellation)
        {
            lock(thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                Next(playingGame, cancellation);

                CurrentPlayingGameRepository.Save(playingGame);
            }
        }
    }
}
