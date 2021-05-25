using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Games
{
    public interface IWinningChecker
    {
        bool IsWinning(Game game, Player player);
    }
    public class TakeKingOrEnterOpponentTerritoryWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, Player player)
        {
            return game.KingEnterOpponentTerritory(player) || !game.State.ExistKing(player.Opponent);
        }
    }
    public class CheckmateWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, Player player)
        {
            return game.DoCheckmate(player);
        }
    }

}
