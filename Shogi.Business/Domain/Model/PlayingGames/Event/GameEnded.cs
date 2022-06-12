using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Event;

namespace Shogi.Business.Domain.Model.PlayingGames.Event
{
    public class GameEnded : IDomainEvent
    {
        public GameEnded(PlayerType winner)
        {
            Winner = winner;
        }

        public PlayerType Winner { get; }
    }
}
