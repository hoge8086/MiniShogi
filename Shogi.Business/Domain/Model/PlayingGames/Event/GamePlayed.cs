using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.Games;

namespace Shogi.Business.Domain.Model.PlayingGames.Event
{
    public class GamePlayed : IDomainEvent
    {
        public GamePlayed(PlayingGame playingGame, MoveCommand moveCommand)
        {
            PlayingGame = playingGame;
            MoveCommand = moveCommand;
        }

        public PlayingGame PlayingGame { get; }
        public MoveCommand MoveCommand { get; }
    }
}
