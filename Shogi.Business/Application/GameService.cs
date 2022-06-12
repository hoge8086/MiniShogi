using System;
using System.Text;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.AI;
using System.Threading;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.PlayingGames;
using System.Collections.Generic;
using Shogi.Business.Domain.Service;
using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.PlayingGames.Event;

namespace Shogi.Business.Application
{

    // [TODO:ゲームの進行をDomainServiceに移す]
    // [MEMO:ApplicationServiceからGetGmae()やGetPlayingGmae()で直接取れないようにすることで、マルチスレッドアクセスを可能にする]
    // [     Clone()したPlayingGameが返る]
    public class GameService
    {
        private Object thisLock = new Object();
        public IGameTemplateRepository GameTemplateRepository;
        public ICurrentPlayingGameRepository CurrentPlayingGameRepository;
        public IPlayingGameRepository PlayingGameRepository;
        private PlayGameService playGameService = new PlayGameService();

        public GameService(IGameTemplateRepository gameTemplateRepository, ICurrentPlayingGameRepository currentPlayingGameRepository, IPlayingGameRepository playingGameRepository)
        {
            GameTemplateRepository = gameTemplateRepository;
            CurrentPlayingGameRepository = currentPlayingGameRepository;
            PlayingGameRepository = playingGameRepository;
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

                var playingGame = CurrentPlayingGameRepository.Current();
                try
                {
                    DomainEvents.Raise(new GameStarted(playingGame.Clone()));
                    playGameService.Next(playingGame, cancellation);
                }
                finally
                {
                    CurrentPlayingGameRepository.Save(playingGame);
                }
            }
        }

        public void Start(List<Player> players, PlayerType firstTurnPlayer, string gameName, CancellationToken cancellation)
        {
            lock (thisLock)
            {
                var template = GameTemplateRepository.FindByName(gameName);
                var game = new GameFactory().Create(template, firstTurnPlayer);
                var playingGame = new PlayingGame(players, game, template);
                try
                {
                    DomainEvents.Raise(new GameStarted(playingGame.Clone()));
                    playGameService.Next(playingGame, cancellation);
                }
                finally
                {
                    CurrentPlayingGameRepository.Save(playingGame);
                }
            }
        }
        //public void Restart(CancellationToken cancellation)
        //{
        //    lock (thisLock)
        //    {
        //        var playingGame = CurrentPlayingGameRepository.Current();
        //        playingGame.Reset();

        //        try
        //        {
        //            DomainEvents.Raise(new GameStarted(playingGame.Clone()));
        //            playGameService.Next(playingGame, cancellation);
        //        }
        //        finally
        //        {
        //            CurrentPlayingGameRepository.Save(playingGame);
        //        }
        //    }
        //}

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
                try
                {
                    playGameService.Play(playingGame, moveCommand, cancellation);
                }finally
                {
                    CurrentPlayingGameRepository.Save(playingGame);
                }
            }
        }

        public void Resume(CancellationToken cancellation)
        {
            lock(thisLock)
            {
                var playingGame = CurrentPlayingGameRepository.Current();
                try
                {
                    playGameService.Next(playingGame, cancellation);
                }finally
                {
                    CurrentPlayingGameRepository.Save(playingGame);
                }
            }
        }
    }
}
