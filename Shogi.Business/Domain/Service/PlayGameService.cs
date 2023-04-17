using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.AI.Event;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayingGames;
using Shogi.Business.Domain.Model.PlayingGames.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Shogi.Business.Domain.Service
{
    /// <summary>
    /// ゲーム進行を行うドメインサービス
    /// </summary>
    public class PlayGameService
    {
        public void Play(PlayingGame playingGame,  MoveCommand moveCommand, CancellationToken cancellation)
        {
            playingGame.Game.Play(moveCommand);
            DomainEvents.Raise(new GamePlayed(playingGame.Clone(), moveCommand));
            Next(playingGame, cancellation);
        }

        /// <summary>
        /// ゲーム進行
        /// </summary>
        /// <param name="playingGame"></param>
        /// <param name="cancellation"></param>
        public void Next(PlayingGame playingGame, CancellationToken cancellation)
        {
            System.Diagnostics.Debug.WriteLine("----------------------");
            System.Diagnostics.Debug.WriteLine(playingGame.Game.ToString());
            if(playingGame.Game.State.IsEnd)
            {
                DomainEvents.Raise(new GameEnded(playingGame.Game.State.GameResult.Winner));
                return;
            }

            if(playingGame.TurnPlayer.IsHuman)
            {
                return;
            }

            try
            {
                DomainEvents.Raise(new ComputerThinkingStarted(playingGame.TurnPlayer.PlayerType));
                var move = playingGame.TurnPlayer.Computer.SelectMove(
                                            playingGame.Game,
                                            cancellation,
                                            (progress) => {
                                                DomainEvents.Raise(new ComputerThinkingProgressed(playingGame.TurnPlayer.PlayerType, progress));
                                            });
                playingGame.Game.Play(move.MoveCommand);
                DomainEvents.Raise(new GamePlayed(playingGame.Clone(), move.MoveCommand));
                DomainEvents.Raise(new ComputerThinkingEnded(playingGame.TurnPlayer.PlayerType, move.GameEvaluation));
            }
            catch(OperationCanceledException ex)
            {
                DomainEvents.Raise(new ComputerThinkingEnded(playingGame.TurnPlayer.PlayerType, null));
                throw;
            }

            Next(playingGame, cancellation);
        }
    }
}
