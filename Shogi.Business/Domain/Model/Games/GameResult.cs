using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.Games
{
    public class GameResult
    {
        public PlayerType Winner { get; private set; }
        public GameResult(PlayerType winner)
        {
            Winner = winner;
        }

    }
}
