using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model
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
