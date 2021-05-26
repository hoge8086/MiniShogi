using Shogi.Business.Domain.Model.Players;

namespace Shogi.Business.Domain.Model.Games
{
    public class GameResult
    {
        public Player Winner { get; private set; }
        public GameResult(Player winner)
        {
            Winner = winner;
        }

    }
}
