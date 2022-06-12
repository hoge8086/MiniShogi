using Shogi.Business.Domain.Event;

namespace Shogi.Business.Domain.Model.PlayingGames.Event
{
    public class GameStarted : IDomainEvent
    {
        public PlayingGame PlayingGame { get; }

        public GameStarted(PlayingGame playingGame)
        {
            PlayingGame = playingGame;
        }
    }
}
